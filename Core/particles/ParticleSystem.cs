using Core.render.shaders;
using Core.Particles;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Core.Particles {
    public class ParticleSystem {

        public const int MaxParticles = 100000;
        private int _activeParticleCount = 0;

        private List<Particle> _particles = new List<Particle>();
        private List<Emitter> _emitters = new List<Emitter>();
        private List<IForceField> _forceFields = new List<IForceField>();

        private float[] _particlePositionSizeData = new float[MaxParticles * 4];
        private byte[] _particleColorData = new byte[MaxParticles * 4];

        private int _quadVBO;
        private int _instanceVBO;
        private int _colorVBO;
        private Shader _shader;

        private int _vao;

        public ParticleSystem(Shader shader) {

            _shader = shader;
            InitializeBuffers();
        }

        private void InitializeBuffers() {
            float[] quadVertices = {
                -0.5f, -0.5f, 0.0f, // Bottom-left
                 0.5f, -0.5f, 0.0f, // Bottom-right
                 0.5f,  0.5f, 0.0f, // Top-right
                -0.5f,  0.5f, 0.0f  // Top-left
            };

            uint[] quadIndices = {
                0, 1, 2,
                2, 3, 0
            };

            // Generate buffers
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            // Quad VBO
            _quadVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            // Position and size VBO (instanced)
            _instanceVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            // Color VBO (instanced)
            _colorVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(byte), IntPtr.Zero, BufferUsageHint.DynamicDraw);

            // Element Buffer Object
            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, quadIndices.Length * sizeof(uint), quadIndices, BufferUsageHint.StaticDraw);

            // Set up vertex attributes
            // Quad vertices
            GL.BindBuffer(BufferTarget.ArrayBuffer, _quadVBO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            // Position and size (instanced)
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVBO);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.VertexAttribDivisor(1, 1); // Update per instance

            // Color (instanced)
            GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVBO);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, 4 * sizeof(byte), 0);
            GL.VertexAttribDivisor(2, 1); // Update per instance

            // Unbind VAO
            GL.BindVertexArray(0);
        }

        public void AddEmitter(Emitter emitter) { _emitters.Add(emitter); }

        public void AddForceField(IForceField forceField) { _forceFields.Add(forceField); }

        public void AddParticles(IEnumerable<Particle> particles) { _particles.AddRange(particles); }


        public void Update(float deltaTime, Vector2 playerPosition) {

            // Emit new particles
            for(int i = _emitters.Count - 1; i >= 0; i--) {
                var emitter = _emitters[i];
                emitter.Emit(_particles, deltaTime);

                if(!emitter.IsActive) {
                    _emitters.RemoveAt(i);
                }
            }

            // Update existing particles
            int particleCount = 0;
            for(int i = _particles.Count - 1; i >= 0; i--) {
                Particle particle = _particles[i];

                // Apply forces
                foreach(var forceField in _forceFields) {
                    forceField.ApplyForce(particle, deltaTime);
                }

    
                
                particle.Update(playerPosition, deltaTime);
                

                if(particle.IsAlive) {
                    if(particleCount >= MaxParticles) {
                        // Avoid exceeding buffer size
                        break;
                    }

                    // Prepare data for rendering
                    Vector4 currentColor = particle.GetCurrentColor();

                    _particlePositionSizeData[4 * particleCount + 0] = particle.Position.X;
                    _particlePositionSizeData[4 * particleCount + 1] = particle.Position.Y;
                    _particlePositionSizeData[4 * particleCount + 2] = particle.GetCurrentSize(); // Size
                    _particlePositionSizeData[4 * particleCount + 3] = 0.0f; // Unused

                    _particleColorData[4 * particleCount + 0] = (byte)(currentColor.X * 255);
                    _particleColorData[4 * particleCount + 1] = (byte)(currentColor.Y * 255);
                    _particleColorData[4 * particleCount + 2] = (byte)(currentColor.Z * 255);
                    _particleColorData[4 * particleCount + 3] = (byte)(currentColor.W * 255);

                    particleCount++;
                }
                else {
                    _particles.RemoveAt(i);
                }
            }

            // Update instance data buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instanceVBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, particleCount * 4 * sizeof(float), _particlePositionSizeData);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _colorVBO);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, particleCount * 4 * sizeof(byte), _particleColorData);

            _activeParticleCount = particleCount;
        }

        public void Render()
        {
            // Don't render if there's nothing to draw
            if (_activeParticleCount == 0) return;

            // Ensure our specific particle shader is active before setting uniforms
            _shader.Use();
            GL.BindVertexArray(_vao);

            // Set shared OpenGL state
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            // ===== PASS 1: DETAIL & CORE (Standard Blending) =====
            // This pass draws the main body of the particle with high contrast.
            _shader.SetUniform("renderPass", 0);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Draw the particles for the first time
            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero, _activeParticleCount);

            // ===== PASS 2: GLOW & SPARKLES (Additive Blending) =====
            // This pass adds a soft, emissive glow and bright sparkles on top.
            _shader.SetUniform("renderPass", 1);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One); // Additive blending for the glow

            // Draw the exact same particles a second time with different shader logic
            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero, _activeParticleCount);

            // Unbind the VAO to be tidy
            GL.BindVertexArray(0);

            // It's good practice to reset the blend func if other parts of your renderer expect a default
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

    }
}

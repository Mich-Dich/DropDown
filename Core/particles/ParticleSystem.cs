using Core.render.shaders;
using Core.Particles;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Core.Particles
{
    public class ParticleSystem
    {
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

        public ParticleSystem(Shader shader)
        {
            _shader = shader;
            InitializeBuffers();
        }

        private void InitializeBuffers()
        {
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

        public void AddEmitter(Emitter emitter)
        {
            _emitters.Add(emitter);
        }

        public void AddForceField(IForceField forceField)
        {
            _forceFields.Add(forceField);
        }

        public void AddParticles(IEnumerable<Particle> particles)
        {
            _particles.AddRange(particles);
        }

        public void Update(float deltaTime, Vector2 playerPosition)
        {
            // Emit new particles
            for (int i = _emitters.Count - 1; i >= 0; i--)
            {
                var emitter = _emitters[i];
                emitter.Emit(_particles, deltaTime);

                if (!emitter.IsActive)
                {
                    _emitters.RemoveAt(i);
                }
            }

            // Update existing particles
            int particleCount = 0;
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                Particle particle = _particles[i];

                // Apply forces
                foreach (var forceField in _forceFields)
                {
                    forceField.ApplyForce(particle, deltaTime);
                }

                if (particle is XPParticle xpParticle)
                {
                    xpParticle.Update(playerPosition, deltaTime);
                }
                else
                {
                    particle.Update(deltaTime);
                }

                if (particle.IsAlive)
                {
                    if (particleCount >= MaxParticles)
                    {
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
                else
                {
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
            GL.BindVertexArray(_vao);

            // Set OpenGL state
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            GL.Disable(EnableCap.DepthTest);

            _shader.Use();

            // Set uniforms
            _shader.Set_Matrix_4x4("projection", Game.Instance.camera.Get_Projection_Matrix());

            // Draw particles
            GL.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero, _activeParticleCount);

            GL.BindVertexArray(0);
        }
    }
}

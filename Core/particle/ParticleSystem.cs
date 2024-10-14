using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using Core.render.shaders;
using Core.util;
using Core.world;

namespace Core.particle
{
    public class ParticleSystem
    {
        private const int MaxParticles = 100000;
        private List<Particle> _particles = new List<Particle>(MaxParticles);
        private List<Emitter> _emitters = new List<Emitter>();
        private float[] _particlePositionSizeData = new float[MaxParticles * 4];
        private byte[] _particleColorData = new byte[MaxParticles * 4];

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _positionBuffer;
        private int _colorBuffer;
        private Shader _shader;

        private Vector2 _gravity = Vector2.Zero;
        private Vector2 _shockwaveGravity = Vector2.Zero;
        private Vector2 _attractionPoint = new Vector2(0, 0);
        private float _attractionStrength = 0.0f;

        private Camera _camera;

        public ParticleSystem(Shader shader, Camera camera)
        {
            _shader = shader;
            _camera = camera;
            InitializeBuffers();
        }

        private void InitializeBuffers()
        {
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            float[] quadVertices = {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                -0.5f,  0.5f, 0.0f,
                0.5f,  0.5f, 0.0f,
            };

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

            _positionBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _positionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);

            _colorBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _colorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(byte), IntPtr.Zero, BufferUsageHint.StreamDraw);

            _shader.Use();

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _positionBuffer);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.VertexAttribDivisor(1, 1);

            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _colorBuffer);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, 4 * sizeof(byte), 0);
            GL.VertexAttribDivisor(2, 1);
        }

        public void AddEmitter(Emitter emitter)
        {
            _emitters.Add(emitter);
        }

        public void Update(float deltaTime)
        {
            int particleCount = 0;
            Random random = new Random();

            foreach (var emitter in _emitters)
            {
                if (emitter.Continuous || _particles.Count == 0)
                {
                    for (int i = 0; i < emitter.EmissionRate * deltaTime; i++)
                    {
                        if (_particles.Count < MaxParticles)
                        {
                            float size = (float)(random.NextDouble() * (emitter.MaxSize - emitter.MinSize) + emitter.MinSize);
                            float rotation = (float)(random.NextDouble() * (emitter.MaxRotation - emitter.MinRotation) + emitter.MinRotation);
                            _particles.Add(new Particle(
                                emitter.Transform.position,
                                emitter.Velocity,
                                emitter.ColorGradient,
                                size,
                                1.0f,
                                rotation
                            ));
                        }
                    }
                }
            }

            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                Particle p = _particles[i];
                if (p.Life > 0.0f)
                {
                    p.Life -= deltaTime;
                    if (p.Life > 0.0f)
                    {
                        if (p.IsShockwave)
                        {
                            p.Velocity += _shockwaveGravity * deltaTime;
                        }
                        else if (_gravity != Vector2.Zero)
                        {
                            p.Velocity += _gravity * deltaTime;
                        }

                        if (!p.IsShockwave)
                        {
                            Vector2 attractionForce = _attractionPoint - p.Position;
                            if (attractionForce.Length > 0)
                            {
                                attractionForce.Normalize();
                                attractionForce *= _attractionStrength;
                                p.Velocity += attractionForce * deltaTime;
                            }
                        }

                        p.Position += p.Velocity * deltaTime;

                        Vector4 currentColor = p.GetCurrentColor();

                        _particlePositionSizeData[4 * particleCount + 0] = p.Position.X;
                        _particlePositionSizeData[4 * particleCount + 1] = p.Position.Y;
                        _particlePositionSizeData[4 * particleCount + 2] = 0.0f;
                        _particlePositionSizeData[4 * particleCount + 3] = p.Size;

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
            }
        }

        public void Render()
        {
            GL.BindVertexArray(_vertexArrayObject);
            _shader.Use();

            // Set the projection matrix uniform
            Matrix4 projectionMatrix = _camera.Get_Projection_Matrix();
            int projectionLocation = GL.GetUniformLocation(_shader.Handle, "projection");
            GL.UniformMatrix4(projectionLocation, false, ref projectionMatrix);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _positionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MaxParticles * 4 * sizeof(float), _particlePositionSizeData);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _colorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(byte), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, MaxParticles * 4 * sizeof(byte), _particleColorData);

            GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, 4, _particles.Count);
        }

        public void SetGravity(Vector2 gravity)
        {
            _gravity = gravity;
        }

        public void SetAttractionPoint(Vector2 point, float strength)
        {
            _attractionPoint = point;
            _attractionStrength = strength;
        }

        public void CreateShockwave(Vector2 position, int particleCount, float speed, float lifetime, float scale)
        {
            Random random = new Random();
            float defaultParticleSize = 0.05f;

            // Define the color gradient for the shockwave
            ColorGradient colorGradient = new ColorGradient();
            colorGradient.AddColor(0.0f, new Vector4(0.9f, 0.9f, 1.0f, 1.0f)); // Bright white-blue
            colorGradient.AddColor(0.3f, new Vector4(0.6f, 0.6f, 1.0f, 1.0f)); // Light blue
            colorGradient.AddColor(0.6f, new Vector4(0.3f, 0.3f, 1.0f, 1.0f)); // Medium blue
            colorGradient.AddColor(1.0f, new Vector4(0.1f, 0.1f, 0.8f, 1.0f)); // Dark blue

            for (int i = 0; i < particleCount; i++)
            {
                float angle = (float)(i * 2 * Math.PI / particleCount);
                float speedVariation = (float)(random.NextDouble() * 0.5 + 0.75); // Vary speed slightly
                float lifetimeVariation = (float)(random.NextDouble() * 0.5 + 0.75); // Vary lifetime slightly
                float sizeVariation = (float)(random.NextDouble() * 0.5 + 0.75); // Vary size slightly
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed * speedVariation * scale;

                Vector4 colorVariation = new Vector4(
                    (float)(random.NextDouble() * 0.05 - 0.025),
                    (float)(random.NextDouble() * 0.05 - 0.025),
                    (float)(random.NextDouble() * 0.05 - 0.025),
                    0.0f
                );

                ColorGradient particleColorGradient = new ColorGradient();
                foreach (var colorPoint in colorGradient.GetColors())
                {
                    particleColorGradient.AddColor(colorPoint.time, colorPoint.color + colorVariation);
                }

                _particles.Add(new Particle(
                    position,
                    velocity,
                    particleColorGradient,
                    defaultParticleSize * sizeVariation * scale,
                    lifetime * lifetimeVariation,
                    0.0f,
                    true
                ));
            }
        }
    }
}
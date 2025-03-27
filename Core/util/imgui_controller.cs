namespace Core.util {

    using ImGuiNET;
    using OpenTK.Graphics.OpenGL4;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Desktop;
    using OpenTK.Windowing.GraphicsLibraryFramework;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;

    public class ImguI_Controller : IDisposable {
        private static bool kHRDebugAvailable = false;
        private readonly List<char> pressedChars = new ();
        private readonly int gLVersion;
        private readonly bool compatibilityProfile;
        private bool frameBegun;

        private int vertexArray;
        private int vertexBuffer;
        private int vertexBufferSize;
        private int indexBuffer;
        private int indexBufferSize;

        private int fontTexture;

        private int shader;
        private int shaderFontTextureLocation;
        private int shaderProjectionMatrixLocation;

        private int windowWidth;
        private int windowHeight;

        private System.Numerics.Vector2 scaleFactor = System.Numerics.Vector2.One;

        public ImguI_Controller(int width, int height, float scaleFactorX = 1, float scaleFactorY = 1) {
            this.windowWidth = width;
            this.windowHeight = height;

            this.scaleFactor.X *= scaleFactorX;
            this.scaleFactor.Y *= scaleFactorY;

            int major = GL.GetInteger(GetPName.MajorVersion);
            int minor = GL.GetInteger(GetPName.MinorVersion);

            this.gLVersion = (major * 100) + (minor * 10);

            kHRDebugAvailable = (major == 4 && minor >= 3) || IsExtensionSupported("KHR_debug");

            this.compatibilityProfile = (GL.GetInteger((GetPName)All.ContextProfileMask) & (int)All.ContextCompatibilityProfileBit) != 0;

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var io = ImGui.GetIO();

            float m_font_size = 15.5f;
            float m_big_font_size = 20.0f;
            Imgui_Fonts.fonts.Add("default", io.Fonts.AddFontFromFileTTF("assets/fonts/Open_Sans/static/OpenSans-Regular.ttf", m_font_size));
            Imgui_Fonts.fonts.Add("bold", io.Fonts.AddFontFromFileTTF("assets/fonts/Open_Sans/static/OpenSans-Bold.ttf", m_font_size));
            Imgui_Fonts.fonts.Add("italic", io.Fonts.AddFontFromFileTTF("assets/fonts/Open_Sans/static/OpenSans-Italic.ttf", m_font_size));

            Imgui_Fonts.fonts.Add("regular_big", io.Fonts.AddFontFromFileTTF("assets/fonts/Open_Sans/static/OpenSans-Regular.ttf", m_big_font_size));
            Imgui_Fonts.fonts.Add("bold_big", io.Fonts.AddFontFromFileTTF("assets/fonts/Open_Sans/static/OpenSans-Bold.ttf", m_big_font_size));
            Imgui_Fonts.fonts.Add("italic_big", io.Fonts.AddFontFromFileTTF("assets/fonts/Open_Sans/static/OpenSans-Italic.ttf", m_big_font_size));

            Imgui_Fonts.fonts.Add("giant", io.Fonts.AddFontFromFileTTF("assets/fonts/Open_Sans/static/OpenSans-Bold.ttf", 30));

            // io.FontDefault = Imgui_Fonts.fonts["default"];
            io.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;
            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            // Enable Docking
            io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;         // Enable Multi-Viewport / Platform Windows
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;       // Enable Keyboard Controls
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;        // Enable Gamepad Controls
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;           // Enable Docking

            this.CreateDeviceResources();

            this.SetPerFrameImGuiData(1f / 60f);

            ImGui.NewFrame();
            this.frameBegun = true;
        }

        // ================================================================= public =================================================================
        public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name) {
            if(kHRDebugAvailable) {
                GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
            }
        }

        public static int CreateProgram(string name, string vertexSource, string fragmentSoruce) {
            int program = GL.CreateProgram();
            LabelObject(ObjectLabelIdentifier.Program, program, $"Program: {name}");

            int vertex = CompileShader(name, ShaderType.VertexShader, vertexSource);
            int fragment = CompileShader(name, ShaderType.FragmentShader, fragmentSoruce);

            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if(success == 0) {
                string info = GL.GetProgramInfoLog(program);
                Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");
            }

            GL.DetachShader(program, vertex);
            GL.DetachShader(program, fragment);

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            return program;
        }

        public static void CheckGLError(string title) {
            ErrorCode error;
            int i = 1;
            while((error = GL.GetError()) != ErrorCode.NoError) {
                Debug.Print($"{title} ({i++}): {error}");
            }
        }

        public static ImGuiKey TranslateKey(Keys key) {
            if(key >= Keys.D0 && key <= Keys.D9) {
                return key - Keys.D0 + ImGuiKey._0;
            }

            if(key >= Keys.A && key <= Keys.Z) {
                return key - Keys.A + ImGuiKey.A;
            }

            if(key >= Keys.KeyPad0 && key <= Keys.KeyPad9) {
                return key - Keys.KeyPad0 + ImGuiKey.Keypad0;
            }

            if(key >= Keys.F1 && key <= Keys.F24) {
                return key - Keys.F1 + ImGuiKey.F24;
            }

            switch(key) {
                case Keys.Tab: return ImGuiKey.Tab;
                case Keys.Left: return ImGuiKey.LeftArrow;
                case Keys.Right: return ImGuiKey.RightArrow;
                case Keys.Up: return ImGuiKey.UpArrow;
                case Keys.Down: return ImGuiKey.DownArrow;
                case Keys.PageUp: return ImGuiKey.PageUp;
                case Keys.PageDown: return ImGuiKey.PageDown;
                case Keys.Home: return ImGuiKey.Home;
                case Keys.End: return ImGuiKey.End;
                case Keys.Insert: return ImGuiKey.Insert;
                case Keys.Delete: return ImGuiKey.Delete;
                case Keys.Backspace: return ImGuiKey.Backspace;
                case Keys.Space: return ImGuiKey.Space;
                case Keys.Enter: return ImGuiKey.Enter;
                case Keys.Escape: return ImGuiKey.Escape;
                case Keys.Apostrophe: return ImGuiKey.Apostrophe;
                case Keys.Comma: return ImGuiKey.Comma;
                case Keys.Minus: return ImGuiKey.Minus;
                case Keys.Period: return ImGuiKey.Period;
                case Keys.Slash: return ImGuiKey.Slash;
                case Keys.Semicolon: return ImGuiKey.Semicolon;
                case Keys.Equal: return ImGuiKey.Equal;
                case Keys.LeftBracket: return ImGuiKey.LeftBracket;
                case Keys.Backslash: return ImGuiKey.Backslash;
                case Keys.RightBracket: return ImGuiKey.RightBracket;
                case Keys.GraveAccent: return ImGuiKey.GraveAccent;
                case Keys.CapsLock: return ImGuiKey.CapsLock;
                case Keys.ScrollLock: return ImGuiKey.ScrollLock;
                case Keys.NumLock: return ImGuiKey.NumLock;
                case Keys.PrintScreen: return ImGuiKey.PrintScreen;
                case Keys.Pause: return ImGuiKey.Pause;
                case Keys.KeyPadDecimal: return ImGuiKey.KeypadDecimal;
                case Keys.KeyPadDivide: return ImGuiKey.KeypadDivide;
                case Keys.KeyPadMultiply: return ImGuiKey.KeypadMultiply;
                case Keys.KeyPadSubtract: return ImGuiKey.KeypadSubtract;
                case Keys.KeyPadAdd: return ImGuiKey.KeypadAdd;
                case Keys.KeyPadEnter: return ImGuiKey.KeypadEnter;
                case Keys.KeyPadEqual: return ImGuiKey.KeypadEqual;
                case Keys.LeftShift: return ImGuiKey.LeftShift;
                case Keys.LeftControl: return ImGuiKey.LeftCtrl;
                case Keys.LeftAlt: return ImGuiKey.LeftAlt;
                case Keys.LeftSuper: return ImGuiKey.LeftSuper;
                case Keys.RightShift: return ImGuiKey.RightShift;
                case Keys.RightControl: return ImGuiKey.RightCtrl;
                case Keys.RightAlt: return ImGuiKey.RightAlt;
                case Keys.RightSuper: return ImGuiKey.RightSuper;
                case Keys.Menu: return ImGuiKey.Menu;
                default: return ImGuiKey.None;
            }
        }

        public void WindowResized(int width, int height) {
            this.windowWidth = (int)(width * this.scaleFactor.X);
            this.windowHeight = (int)(height * this.scaleFactor.Y);
        }

        public void DestroyDeviceObjects() {
            this.Dispose();
        }

        public void CreateDeviceResources() {
            this.vertexBufferSize = 10000;
            this.indexBufferSize = 2000;

            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);

            this.vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArray);
            LabelObject(ObjectLabelIdentifier.VertexArray, this.vertexArray, "ImGui");

            this.vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
            LabelObject(ObjectLabelIdentifier.Buffer, this.vertexBuffer, "VBO: ImGui");
            GL.BufferData(BufferTarget.ArrayBuffer, this.vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            this.indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer);
            LabelObject(ObjectLabelIdentifier.Buffer, this.indexBuffer, "EBO: ImGui");
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            this.RecreateFontDeviceTexture();

            string vertexSource = @"#version 330 core

uniform mat4 projection_matrix;

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;

out vec4 color;
out vec2 texCoord;

void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}";
            string fragmentSource = @"#version 330 core

uniform sampler2D in_fontTexture;

in vec4 color;
in vec2 texCoord;

out vec4 outputColor;

void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}";

            this.shader = CreateProgram("ImGui", vertexSource, fragmentSource);
            this.shaderProjectionMatrixLocation = GL.GetUniformLocation(this.shader, "projection_matrix");
            this.shaderFontTextureLocation = GL.GetUniformLocation(this.shader, "in_fontTexture");

            int stride = Unsafe.SizeOf<ImDrawVert>();
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(prevVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);

            CheckGLError("End of ImGui setup");
        }

        public void RecreateFontDeviceTexture() {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            int mips = (int)Math.Floor(Math.Log(Math.Max(width, height), 2));

            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);

            this.fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, this.fontTexture);
            GL.TexStorage2D(TextureTarget2d.Texture2D, mips, SizedInternalFormat.Rgba8, width, height);
            LabelObject(ObjectLabelIdentifier.Texture, this.fontTexture, "ImGui Text Atlas");

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mips - 1);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            // Restore state
            GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);

            io.Fonts.SetTexID((IntPtr)this.fontTexture);

            io.Fonts.ClearTexData();
        }

        public void Render() {
            if(this.frameBegun) {
                this.frameBegun = false;
                ImGui.Render();
                this.RenderImDrawData(ImGui.GetDrawData());
            }
        }

        public void Update(GameWindow wnd, float deltaSeconds) {
            if(this.frameBegun) {
                ImGui.Render();
            }

            this.SetPerFrameImGuiData(deltaSeconds);
            this.UpdateImGuiInput(wnd);

            this.frameBegun = true;
            ImGui.NewFrame();
        }

        public void Dispose() {
            GL.DeleteVertexArray(this.vertexArray);
            GL.DeleteBuffer(this.vertexBuffer);
            GL.DeleteBuffer(this.indexBuffer);

            GL.DeleteTexture(this.fontTexture);
            GL.DeleteProgram(this.shader);
        }

        // ================================================================= internal =================================================================
        internal void PressChar(char keyChar) {
            this.pressedChars.Add(keyChar);
        }

        internal void MouseScroll(Vector2 offset) {
            ImGuiIOPtr io = ImGui.GetIO();
            io.MouseWheel = offset.Y;
            io.MouseWheelH = offset.X;
        }

        // ================================================================= statis =================================================================
        private static bool IsExtensionSupported(string name) {
            int n = GL.GetInteger(GetPName.NumExtensions);
            for(int i = 0; i < n; i++) {
                string extension = GL.GetString(StringNameIndexed.Extensions, i);
                if(extension == name) {
                    return true;
                }
            }

            return false;
        }

        private static int CompileShader(string name, ShaderType type, string source) {
            int shader = GL.CreateShader(type);
            LabelObject(ObjectLabelIdentifier.Shader, shader, $"Shader: {name}");

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if(success == 0) {
                string info = GL.GetShaderInfoLog(shader);
                Debug.WriteLine($"GL.CompileShader for shader '{name}' [{type}] had info log:\n{info}");
            }

            return shader;
        }

        // ================================================================= private =================================================================
        private void SetPerFrameImGuiData(float deltaSeconds) {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                this.windowWidth / this.scaleFactor.X,
                this.windowHeight / this.scaleFactor.Y);
            io.DisplayFramebufferScale = this.scaleFactor;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        private void UpdateImGuiInput(GameWindow wnd) {
            ImGuiIOPtr io = ImGui.GetIO();
            MouseState mouseState = wnd.MouseState;
            KeyboardState keyboardState = wnd.KeyboardState;

            io.MouseDown[0] = mouseState[MouseButton.Left];
            io.MouseDown[1] = mouseState[MouseButton.Right];
            io.MouseDown[2] = mouseState[MouseButton.Middle];
            io.MouseDown[3] = mouseState[MouseButton.Button4];
            io.MouseDown[4] = mouseState[MouseButton.Button5];

            var screenPoint = new Vector2i((int)mouseState.X, (int)mouseState.Y);
            var point = screenPoint; // wnd.PointToClient(screenPoint);
            io.MousePos = new System.Numerics.Vector2(point.X, point.Y);

            foreach(Keys key in Enum.GetValues(typeof(Keys))) {
                if(key == Keys.Unknown) {
                    continue;
                }

                io.AddKeyEvent(TranslateKey(key), keyboardState.IsKeyDown(key));
            }

            foreach(var c in this.pressedChars) {
                io.AddInputCharacter(c);
            }

            this.pressedChars.Clear();

            io.KeyCtrl = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
            io.KeyShift = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            io.KeySuper = keyboardState.IsKeyDown(Keys.LeftSuper) || keyboardState.IsKeyDown(Keys.RightSuper);
        }

        private void RenderImDrawData(ImDrawDataPtr draw_data) {
            if(draw_data.CmdListsCount == 0) {
                return;
            }

            // Get intial state.
            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
            int prevProgram = GL.GetInteger(GetPName.CurrentProgram);
            bool prevBlendEnabled = GL.GetBoolean(GetPName.Blend);
            bool prevScissorTestEnabled = GL.GetBoolean(GetPName.ScissorTest);
            int prevBlendEquationRgb = GL.GetInteger(GetPName.BlendEquationRgb);
            int prevBlendEquationAlpha = GL.GetInteger(GetPName.BlendEquationAlpha);
            int prevBlendFuncSrcRgb = GL.GetInteger(GetPName.BlendSrcRgb);
            int prevBlendFuncSrcAlpha = GL.GetInteger(GetPName.BlendSrcAlpha);
            int prevBlendFuncDstRgb = GL.GetInteger(GetPName.BlendDstRgb);
            int prevBlendFuncDstAlpha = GL.GetInteger(GetPName.BlendDstAlpha);
            bool prevCullFaceEnabled = GL.GetBoolean(GetPName.CullFace);
            bool prevDepthTestEnabled = GL.GetBoolean(GetPName.DepthTest);
            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);
            Span<int> prevScissorBox = stackalloc int[4];
            unsafe {
                fixed(int* iptr = &prevScissorBox[0]) {
                    GL.GetInteger(GetPName.ScissorBox, iptr);
                }
            }

            Span<int> prevPolygonMode = stackalloc int[2];
            unsafe {
                fixed(int* iptr = &prevPolygonMode[0]) {
                    GL.GetInteger(GetPName.PolygonMode, iptr);
                }
            }

            if(this.gLVersion <= 310 || this.compatibilityProfile) {
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
                GL.PolygonMode(MaterialFace.Back, PolygonMode.Fill);
            }
            else {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            // Bind the element buffer (thru the VAO) so that we can resize it.
            GL.BindVertexArray(this.vertexArray);

            // Bind the vertex buffer so that we can resize it.
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBuffer);
            for(int i = 0; i < draw_data.CmdListsCount; i++) {
                ImDrawListPtr cmd_list = draw_data.CmdLists[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if(vertexSize > this.vertexBufferSize) {
                    int newSize = (int)Math.Max(this.vertexBufferSize * 1.5f, vertexSize);

                    GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    this.vertexBufferSize = newSize;

                    Console.WriteLine($"Resized dear imgui vertex buffer to new size {this.vertexBufferSize}");
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if(indexSize > this.indexBufferSize) {
                    int newSize = (int)Math.Max(this.indexBufferSize * 1.5f, indexSize);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    this.indexBufferSize = newSize;

                    Console.WriteLine($"Resized dear imgui index buffer to new size {this.indexBufferSize}");
                }
            }

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
                0.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            GL.UseProgram(this.shader);
            GL.UniformMatrix4(this.shaderProjectionMatrixLocation, false, ref mvp);
            GL.Uniform1(this.shaderFontTextureLocation, 0);
            CheckGLError("Projection");

            GL.BindVertexArray(this.vertexArray);
            CheckGLError("VAO");

            draw_data.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            // Render command lists
            for(int n = 0; n < draw_data.CmdListsCount; n++) {
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];

                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
                CheckGLError($"Data Vert {n}");

                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);
                CheckGLError($"Data Idx {n}");

                for(int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++) {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if(pcmd.UserCallback != IntPtr.Zero) {
                        throw new NotImplementedException();
                    }
                    else {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);
                        CheckGLError("Texture");

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, this.windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                        CheckGLError("Scissor");

                        if((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0) {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(pcmd.IdxOffset * sizeof(ushort)), unchecked((int)pcmd.VtxOffset));
                        }
                        else {
                            GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }

                        CheckGLError("Draw");
                    }
                }
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);

            // Reset state
            GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);
            GL.UseProgram(prevProgram);
            GL.BindVertexArray(prevVAO);
            GL.Scissor(prevScissorBox[0], prevScissorBox[1], prevScissorBox[2], prevScissorBox[3]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);
            GL.BlendEquationSeparate((BlendEquationMode)prevBlendEquationRgb, (BlendEquationMode)prevBlendEquationAlpha);
            GL.BlendFuncSeparate(
                (BlendingFactorSrc)prevBlendFuncSrcRgb,
                (BlendingFactorDest)prevBlendFuncDstRgb,
                (BlendingFactorSrc)prevBlendFuncSrcAlpha,
                (BlendingFactorDest)prevBlendFuncDstAlpha);
            if(prevBlendEnabled) {
                GL.Enable(EnableCap.Blend);
            }
            else {
                GL.Disable(EnableCap.Blend);
            }

            if(prevDepthTestEnabled) {
                GL.Enable(EnableCap.DepthTest);
            }
            else {
                GL.Disable(EnableCap.DepthTest);
            }

            if(prevCullFaceEnabled) {
                GL.Enable(EnableCap.CullFace);
            }
            else {
                GL.Disable(EnableCap.CullFace);
            }

            if(prevScissorTestEnabled) {
                GL.Enable(EnableCap.ScissorTest);
            }
            else {
                GL.Disable(EnableCap.ScissorTest);
            }

            if(this.gLVersion <= 310 || this.compatibilityProfile) {
                GL.PolygonMode(MaterialFace.Front, (PolygonMode)prevPolygonMode[0]);
                GL.PolygonMode(MaterialFace.Back, (PolygonMode)prevPolygonMode[1]);
            }
            else {
                GL.PolygonMode(MaterialFace.FrontAndBack, (PolygonMode)prevPolygonMode[0]);
            }
        }
    }
}
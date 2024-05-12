namespace Core.render {

    public interface I_Buffer {
    
        int id { get; }

        void Bind();

        void Unbind();

    }
}

namespace Core.render {

    public interface i_buffer {
    
        int id { get; }

        void bind();

        void unbind();

    }
}

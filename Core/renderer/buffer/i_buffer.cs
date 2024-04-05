using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.renderer {

    public interface i_buffer {
    
        int id { get; }

        void bind();

        void unbind();

    }
}

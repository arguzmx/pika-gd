using System.Collections.Generic;

namespace RepositorioEntidades
{
    public class CacheBusqueda
    {
        public string Id { get; set; }
        public List<string> Unicos { get; set; }
        public List<string> UnicosElastic { get; set; }
        public string  sort_col { get; set; }
        public string sort_dir { get; set; }

    }
}

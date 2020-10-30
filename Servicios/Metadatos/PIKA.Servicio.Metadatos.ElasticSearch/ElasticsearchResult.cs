using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public class ElasticsearchResult
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public ShardResult _shards { get; set; }
       public HitResult hits { get; set; }


    }

    public class HitTotal
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

    public class HitResult
    {
        public HitTotal total { get; set; }
        public dynamic[] hits { get; set; }
        public decimal? max_score { get; set; }
    }


    public class ShardResult {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

}

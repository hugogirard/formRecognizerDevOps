using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Shared.Models;

public class DocumentInfo
{
    public string ModelId { get; set; }

    public MODEL_ENVIRONMENT Environment { get; set; }

    public string DocumentUrl { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlissBase.Model
{
    public class SuggestionsModel
    {
        public int symbolId { get; set; }
        public List<CompositeSymbol> currentSuggestions { get; set; }
        public string first { get; set; }

    }
}

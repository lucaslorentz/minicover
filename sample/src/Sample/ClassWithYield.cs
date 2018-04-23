using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample
{
    public class ClassWithYield
    {
        public void Execute() {
            foreach(var n in Enumerate1To10()) {

            }
        }

        public IEnumerable<int> Enumerate1To10() {
            for (var i = 1; i <= 10; i++) {
                yield return i;
            }
        }
    }
}

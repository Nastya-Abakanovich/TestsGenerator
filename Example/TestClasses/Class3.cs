using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClasses
{

    public interface IInterface
    {
        int Work();
    }

    public class Class3
    {
        private IInterface currInterface;

        public Class3(IInterface _interface)
        {
            currInterface = _interface;
        }

        public int Method1(int i)
        {
            return i + currInterface.Work();
        }


        public int Method2(int x, int y)
        {
            return 0;
        }

    }

    public class Class4
    {
        private IInterface currInterface;
        private string s;
        private int i;

        public Class4(IInterface _interface, string s, int i)
        {
            currInterface = _interface;
            this.s = s;
            this.i = i;
        }

        public int Method(int i)
        {
            return i + currInterface.Work();
        }

    }
}

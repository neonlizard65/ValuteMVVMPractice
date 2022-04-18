using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuteMVVMPractice.Interfaces
{
    public interface IExportable
    {
        void ExportToExcel();
        void ExportToPDF();
        void ExportToWord();
    }
}

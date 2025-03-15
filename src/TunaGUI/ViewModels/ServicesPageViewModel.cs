using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunaGUI.Data;

namespace TunaGUI.ViewModels;

public partial class ServicesPageViewModel: PageViewModel
{
    public string Test { get; set; } = "Services";

    public ServicesPageViewModel()
    {
        PageName = ApplicationPageNames.Services;

    }
}

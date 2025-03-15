using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunaGUI.Data;
namespace TunaGUI.ViewModels;

public partial class HomePageViewModel: PageViewModel
{
    public string Test { get; set; } = "Home";

    public HomePageViewModel()
    {
        PageName = ApplicationPageNames.Home;
    }
}

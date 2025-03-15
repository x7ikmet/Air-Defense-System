using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunaGUI.ViewModels;
using TunaGUI.Data;

namespace TunaGUI.Factories;

public class PageFactory(Func<ApplicationPageNames, PageViewModel> factory)
{


    public PageViewModel GetPageViewModel(ApplicationPageNames pageName) => factory.Invoke(pageName);
    

}

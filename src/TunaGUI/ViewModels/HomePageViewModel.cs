using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunaGUI.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TunaGUI.ViewModels
{
    public partial class HomePageViewModel : PageViewModel
    {
        [ObservableProperty]
        private WebcamViewModel _webcamViewModel;
        private bool _isActivated = false;

        public HomePageViewModel(WebcamViewModel webcamViewModel)
        {
            if (webcamViewModel == null)
            {
                throw new ArgumentNullException(nameof(webcamViewModel), 
                    "WebcamViewModel dependency cannot be null. Ensure proper DI setup.");
            }
            
            PageName = ApplicationPageNames.Home;
            WebcamViewModel = webcamViewModel; // Use the property, not the field
        }

        // Changed to async Task to properly await the async call
        public async Task OnActivated()
        {
            if (!_isActivated)
            {
                _isActivated = true;
                // Add await to properly wait for this async operation to complete
                await WebcamViewModel.RestartIfNeeded(); // Use the property, not the field
            }
        }
    }
}

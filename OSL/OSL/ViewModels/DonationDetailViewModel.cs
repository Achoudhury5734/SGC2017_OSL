using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OSL.Models;
using Xamarin.Forms;
using OSL.Services;

namespace OSL.ViewModels
{
    public class DonationDetailViewModel : ViewModelBase
    {
        DonationRepository donationRepository;

        public DonationDetailViewModel(Donation item)
        {
            Title = item?.Title;
            Item = item;

            donationRepository = new DonationRepository();

            CompleteCommand = new Command(async () => await CompleteDonationAsync(item.Id), () => item.Status != DonationStatus.Completed);
            WasteCommand = new Command(async () => await WasteDonationAsync(item.Id), () => item.Status != DonationStatus.Wasted);

        }
        public Donation Item { get; set; }

        private async Task CompleteDonationAsync(int donationId)
        {
            await donationRepository.CompleteDonationAsync(donationId);
            await Page.Navigation.PopAsync();
        }

        private async Task WasteDonationAsync(int donationId)
        {
            await donationRepository.WasteDonationAsync(donationId);
            await Page.Navigation.PopAsync();
        }

        private Page page;
        public Page Page
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }

        public Command CompleteCommand { get; set; }
        public Command WasteCommand { get; set; }
    }
}

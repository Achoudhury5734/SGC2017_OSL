using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OSL.Models;
using Xamarin.Forms;
using OSL.Services;
using OSL.Views;

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
            RelistCommand = new Command(async () => await RelistDonationAsync(item.Id), () => CanRelistDonation(item.Status));

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

        private async Task RelistDonationAsync(int donationId)
        {
            await Page.Navigation.PushAsync(new DonationPage(donationId));
        }

        private Page page;
        public Page Page
        {
            get { return page; }
            set { SetProperty(ref page, value); }
        }

        public Command CompleteCommand { get; set; }
        public Command WasteCommand { get; set; }
        public Command RelistCommand { get; set; }

        private bool CanCompleteDonation(DonationStatus status)
        {
            return status == DonationStatus.PendingPickup;
        }

        private bool CanWasteDonation(DonationStatus status)
        {
            return status != DonationStatus.Wasted && status != DonationStatus.Completed; 
        }

        private bool CanRelistDonation(DonationStatus status)
        {
            return status != DonationStatus.Completed;
        }
    }
}

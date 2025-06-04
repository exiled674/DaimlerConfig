namespace DaimlerConfig
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        protected override void OnAppearing()


        {


            base.OnAppearing();


            this.Window.MinimumWidth = 800;


            this.Window.MinimumHeight = 600;


        }
    }
}

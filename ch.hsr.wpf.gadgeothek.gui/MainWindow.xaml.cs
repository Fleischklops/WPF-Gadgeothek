using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;

namespace ch.hsr.wpf.gadgeothek.gui
{
    public partial class MainWindow : Window
    {
        LibraryAdminService libserv;

        public ObservableCollection<Gadget> GadgetList;
        public ObservableCollection<Loan> LoanList;
        public ObservableCollection<Reservation> ReservationList;

        private int NextInventoryNumber;

        public MainWindow()
        {
            InitializeComponent();

            Initialisation();
        }

        private void Initialisation()
        {
            libserv = new LibraryAdminService("http://mge2.dev.ifs.hsr.ch/");

            GetDataFromServer();
            
            comboBox_GadgetCondition.ItemsSource = Enum.GetValues(typeof(domain.Condition));
        }

        private void GetDataFromServer()
        {
            RefreshGadget();
            RefreshLoan();
            RefreshReservation();
        }

        private void RefreshGadget()
        {
            GadgetList = GetGadgetFromServer();
            dataGrid_Gadget.ItemsSource = GadgetList;

            NextInventoryNumber = Convert.ToInt32(GadgetList.LastOrDefault<Gadget>().InventoryNumber) + 1;
        }

        private void RefreshLoan()
        {
            LoanList = GetLoanFromServer();
            dataGrid_Loan.ItemsSource = LoanList;
        }

        private void RefreshReservation()
        {
            ReservationList = GetReservationFromServer();
            dataGrid_Reservation.ItemsSource = ReservationList;
        }

        private ObservableCollection<Gadget> GetGadgetFromServer()
        {
            return new ObservableCollection<Gadget>(libserv.GetAllGadgets());
        }

        private ObservableCollection<Loan> GetLoanFromServer()
        {
            return new ObservableCollection<Loan>(libserv.GetAllLoans());
        }

        private ObservableCollection<Reservation> GetReservationFromServer()
        {
            return new ObservableCollection<Reservation>(libserv.GetAllReservations());
        }

        private void Button_AddGadget_Click(object sender, RoutedEventArgs e)
        {
            domain.Condition cond;
            Enum.TryParse<domain.Condition>(comboBox_GadgetCondition.SelectedValue.ToString(), out cond);

            libserv.AddGadget(new Gadget(textBox_GadgetName.Text) { InventoryNumber = NextInventoryNumber.ToString(), Manufacturer = textBox_GadgetManufactur.Text, Price = Convert.ToDouble(textBox_GadgetPrice.Text), Condition = cond });
            RefreshGadget();
        }
    }
}

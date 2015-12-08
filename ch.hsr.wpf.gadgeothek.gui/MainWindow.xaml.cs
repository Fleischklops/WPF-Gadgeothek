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
        System.Windows.Threading.DispatcherTimer refreshTimer = new System.Windows.Threading.DispatcherTimer();
       
        LibraryAdminService libserv;

        public ObservableCollection<Gadget> GadgetList;
        public ObservableCollection<Loan> LoanList;
        public ObservableCollection<Reservation> ReservationList;

        private int NextInventoryNumber = 0;

        public MainWindow()
        {
            InitializeComponent();

            Initialisation();
        }

        private void Initialisation()
        {
            libserv = new LibraryAdminService("http://mge2.dev.ifs.hsr.ch/");

            GadgetList = new ObservableCollection<Gadget>();
            dataGrid_Gadget.ItemsSource = GadgetList;

            LoanList = new ObservableCollection<Loan>();
            dataGrid_Loan.ItemsSource = LoanList;

            ReservationList = new ObservableCollection<Reservation>();
            dataGrid_Reservation.ItemsSource = ReservationList;

            refreshTimer.Tick += new EventHandler(refreshTimer_Tick);
            refreshTimer.Interval = new TimeSpan(0, 0, 1);
            refreshTimer.Start();
            
            comboBox_GadgetCondition.ItemsSource = Enum.GetValues(typeof(domain.Condition));
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            GetDataFromServer();
        }

        private void GetDataFromServer()
        {
            RefreshGadget();
            RefreshLoan();
            RefreshReservation();
        }

        private void RefreshGadget()
        {
            ObservableCollection<Gadget> newList = GetGadgetFromServer();
            foreach (Gadget g in newList)
            {
                if (!GadgetList.Contains(g))
                {
                    GadgetList.Add(g);
                }
            }
            ObservableCollection<Gadget> toRemove = new ObservableCollection<Gadget>();
            foreach (Gadget g in GadgetList)
            {
                if (!newList.Contains(g))
                {
                    toRemove.Add(g);
                }
            }
            foreach (Gadget g in toRemove)
            {
                GadgetList.Remove(g);
            }

            try
            {
                NextInventoryNumber = Convert.ToInt32(GadgetList.LastOrDefault<Gadget>().InventoryNumber) + 1;
            }
            catch { }
        }

        private void RefreshLoan()
        {
            ObservableCollection<Loan> newList = GetLoanFromServer();
            foreach (Loan l in newList)
            {
                if (!LoanList.Contains(l))
                {
                    LoanList.Add(l);
                }
            }
            ObservableCollection<Loan> toRemove = new ObservableCollection<Loan>();
            foreach (Loan l in LoanList)
            {
                if (!newList.Contains(l))
                {
                    toRemove.Add(l);
                }
            }
            foreach (Loan l in toRemove)
            {
                LoanList.Remove(l);
            }
        }

        private void RefreshReservation()
        {
            ObservableCollection<Reservation> newList = GetReservationFromServer();
            foreach (Reservation r in newList)
            {
                if (!ReservationList.Contains(r))
                {
                    ReservationList.Add(r);
                }
            }
            ObservableCollection<Reservation> toRemove = new ObservableCollection<Reservation>();
            foreach (Reservation r in ReservationList)
            {
                if (!newList.Contains(r))
                {
                    toRemove.Add(r);
                }
            }
            foreach (Reservation r in toRemove)
            {
                ReservationList.Remove(r);
            }
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

        private void ContextMenu_DataGridGadget_Delete_Click(object sender, RoutedEventArgs e)
        {
            Gadget g = GadgetList.ElementAt(dataGrid_Gadget.SelectedIndex);
            GadgetList.Remove(g);

            libserv.DeleteGadget(g);
        }
    }
}

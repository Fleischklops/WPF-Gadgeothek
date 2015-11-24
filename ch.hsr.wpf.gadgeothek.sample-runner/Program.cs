using System;
using System.Threading;
using System.Threading.Tasks;
using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;


namespace ch.hsr.wpf.gadgeothek.runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://mge1.dev.ifs.hsr.ch/";

            // enable low-level HTTP-call logging by uncommenting the following line:
            //RestServiceBase.IsLogging = true;
            

            var sample = new SampleUsage(url);

            // shows simple querying of all the objects
            sample.ShowInventory();

            // shows interaction using the type specific api methods
            sample.ShowAdminInteraction();
            sample.ShowAdminInteractionForLoans();

            // shows the generic interface
            var ipad = new Gadget("iPad Pro") {Manufacturer = "Apple"};
            sample.ShowAdminInteractionWithGenericInterface(
                ipad,
                // get id (inventory number)
                x => x.InventoryNumber,
                // change something
                x => x.Condition = Condition.Waste);

            var max = new Customer("Moritz", "12345", "moritz@hsr.ch", "20151113");
            sample.ShowAdminInteractionWithGenericInterface(
                max,
                // get id (student number)
                x => x.Studentnumber,
                // change something
                x => x.Name = "Moritz der 1.");
            

            sample.ShowUserInteraction();


            Console.WriteLine("<Press Any Key to Terminate the App>");
            Console.ReadKey();
        }
    }
}

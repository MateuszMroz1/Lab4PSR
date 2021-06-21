using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using System.Collections;

namespace CosmosGettingStartedTutorial
{
    class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "Cinema";
        private string containerId = "Items";

        // <Main>
        public static async Task Main(string[] args)
        {
            Program p = new Program();
            try
            {
                Console.WriteLine("Beginning operations...\n");
              
                await p.GetStartedDemoAsync();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }

            int menu;

            while (true)
            {
                Console.WriteLine("Główne Menu:\n");
                Console.WriteLine("\n");
                Console.WriteLine("1.Dodawanie do bazy danych.\n");
                Console.WriteLine("2.Usuwanie całej bazy danych.\n");
                Console.WriteLine("3.Usuwanie rekordu z bazy danych o podanym id.\n");
                Console.WriteLine("4.Wyświetlenie całej bazy danych.\n");
                Console.WriteLine("5.Wyświetlenie rekordu z bazy danych o podanej nazwie kina.\n");
                Console.WriteLine("6.Modyfikacja rekordu o podanym id.\n");
                Console.WriteLine("0.Wyjście z programu.\n");

                Console.WriteLine("Wybierz funkcje. I kliknij klawisz ENTER: ");
                menu = Convert.ToInt32(Console.ReadLine());

                if (menu == 1)
                {
                    await p.AddItemsToContainerAsync();
                    Console.WriteLine("\n");
                }

                if (menu == 2)
                {
                    await p.DeleteDatabaseAndCleanupAsync();
                    Console.WriteLine("Usunięto wszystkie dane z bazy danych.\n");
                    Console.WriteLine("\n");
                }

                if (menu == 3)
                {
                    Console.WriteLine("Wyświetlanie wszystkich danych:\n");
                    await p.ShowAllItemsAsync();
                    Console.WriteLine("\n");
                    await p.DeleteCinemaItemAsyncByID();
                }


                if (menu == 4)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Wyświetlanie wszystkich danych:\n");
                    await p.ShowAllItemsAsync();
                    Console.WriteLine("\n");
                }

                if (menu == 5)
                {
                    await p.ShowItemsAsyncByCinemaName();
                    Console.WriteLine("\n");
                }

                if (menu == 6)
                {
                    await p.ShowAllItemsAsync();
                    Console.WriteLine("\n");

                    await p.UpadateCinemaItemAsync();
                    Console.WriteLine("\n");
                }

                if (menu == 0)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Wyłączanie programu...\n");

                    Environment.Exit(0);  
                }


                if (menu < 0 || menu > 6)
                {
                    Console.WriteLine("\n");
                    Console.WriteLine("Nie ma takiej opcji!\n");
                    Console.WriteLine("\n");
                    continue;
                }
            }
        }

      
        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "AzureCosmosDB" });
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.ScaleContainerAsync();
        }
      
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        private async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }
     
        private async Task ScaleContainerAsync()
        {
            try
            {
                int? throughput = await this.container.ReadThroughputAsync();
                if (throughput.HasValue)
                {
                    Console.WriteLine("Current provisioned throughput : {0}\n", throughput.Value);
                    int newThroughput = throughput.Value + 100;
                   
                    await this.container.ReplaceThroughputAsync(newThroughput);
                    Console.WriteLine("New provisioned throughput : {0}\n", newThroughput);
                }
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.BadRequest)
            {
                Console.WriteLine("Cannot read container throuthput.");
                Console.WriteLine(cosmosException.ResponseBody);
            }
            
        }
       
        private async Task AddItemsToContainerAsync()
        {
            Random rnd = new Random();

            String id = rnd.Next().ToString();
            String partitionKey = rnd.Next().ToString();

            Console.WriteLine("Menu dodawania do bazy danych:\n");

            Console.WriteLine("Podaj nazwe filmu: ");
            String movieName = Console.ReadLine();

            Console.WriteLine("Podaj cene: ");
            int priceD = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Podaj nazwe kina: ");
            String cinemaName = Console.ReadLine();

            CinemaDB cinema = new CinemaDB
            {
                Id = id,
                PartitionKey = partitionKey,
                cinemaName = cinemaName,
                Movies = new Movie[]
                {
                    new Movie {movieName = movieName, price = priceD}
                }
            };

            try
            { 
                ItemResponse<CinemaDB> cinemaResponse = await this.container.ReadItemAsync<CinemaDB>(cinema.Id.ToString(), new PartitionKey(cinema.PartitionKey));
                Console.WriteLine("Item in database with id: {0} already exists\n", cinemaResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<CinemaDB> cinemaResponse = await this.container.CreateItemAsync<CinemaDB>(cinema, new PartitionKey(cinema.PartitionKey));

                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", cinemaResponse.Resource.Id, cinemaResponse.RequestCharge);
            }
        }

        private async Task ShowAllItemsAsync()
        {
            var sqlQueryText = "SELECT * FROM c";

          //  Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<CinemaDB> queryResultSetIterator = this.container.GetItemQueryIterator<CinemaDB>(queryDefinition);

            List<CinemaDB> cinemaDBs = new List<CinemaDB>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<CinemaDB> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (CinemaDB cinema in currentResultSet)
                {
                    cinemaDBs.Add(cinema);
                    Console.WriteLine("\tRead {0}. Cena w zł.\n", cinema);
                }
            }
        }

        private async Task ShowItemsAsyncByCinemaName()
        {

            Console.WriteLine("Podaj nazwe kina: ");
            String cinemaName = Console.ReadLine();

            Console.WriteLine("Wyświetlanie kina o podanej nazwie:\n");

            var sqlQueryText = "SELECT * FROM c Where c.cinemaName = \'" + cinemaName + "\'";

            //  Console.WriteLine("Running query: {0}\n", sqlQueryText);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<CinemaDB> queryResultSetIterator = this.container.GetItemQueryIterator<CinemaDB>(queryDefinition);

            List<CinemaDB> cinemaDBs = new List<CinemaDB>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<CinemaDB> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (CinemaDB cinema in currentResultSet)
                {
                    cinemaDBs.Add(cinema);
                    Console.WriteLine("\tRead {0}. Cena w zł.\n", cinema);
                }
            }
        }

        private async Task UpadateCinemaItemAsync()
        {
            Console.WriteLine("Podaj id do edycji: ");
            String cinemaId = Console.ReadLine(); 
            
            Console.WriteLine("Podaj klucz partycji do edycji: ");
            String partitionKeyValue = Console.ReadLine();

            ItemResponse<CinemaDB> itemResponseCinema = await this.container.ReadItemAsync<CinemaDB>(cinemaId, new PartitionKey(partitionKeyValue));
            var itemBodyCinema = itemResponseCinema.Resource;

            Console.WriteLine("Podaj nazwe kina: ");
            String cinemaNameU = Console.ReadLine();

            itemBodyCinema.cinemaName = cinemaNameU;

            Console.WriteLine("Podaj nazwe filmu: ");
            String movieNameU = Console.ReadLine();

            Console.WriteLine("Podaj cene: ");
            int priceU = Convert.ToInt32(Console.ReadLine());


            itemBodyCinema.Movies[0].movieName = movieNameU;
            itemBodyCinema.Movies[0].price = priceU;

            itemResponseCinema = await this.container.ReplaceItemAsync<CinemaDB>(itemBodyCinema, itemBodyCinema.Id.ToString(), new PartitionKey(itemBodyCinema.PartitionKey));

            Console.WriteLine("Zmienono rekord: ");
            await ShowAllItemsAsync();
        }
       
        private async Task DeleteCinemaItemAsyncByID()
        {
            Console.WriteLine("Podaj id do usunięcia: ");
            String cinemaId = Console.ReadLine();

            Console.WriteLine("Podaj klucz partycji do usunięcia: ");
            String partitionKeyValue = Console.ReadLine();

            ItemResponse<CinemaDB> cinemaResponse = await this.container.DeleteItemAsync<CinemaDB>(cinemaId, new PartitionKey(partitionKeyValue));
            Console.WriteLine("Deleted cinema [{0},{1}]\n", partitionKeyValue, cinemaId);
        }
       
        private async Task DeleteDatabaseAndCleanupAsync()
        {
            DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", this.databaseId);

            await GetStartedDemoAsync();
        }
        
    }
}

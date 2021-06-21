package pl.kielce.tu.neo4j.ogm;

import java.io.IOException;
import java.util.Map;
import java.util.Scanner;

import org.neo4j.ogm.config.Configuration;
import org.neo4j.ogm.session.Session;
import org.neo4j.ogm.session.SessionFactory;

public class TestNeo4J {

	public static void main(String[] args) throws IOException {
		Scanner scan = new Scanner(System.in);
		Configuration configuration = new Configuration.Builder().uri("bolt://localhost:7687").credentials("neo4j", "neo4jpassword").build();
	    SessionFactory sessionFactory = new SessionFactory(configuration, "pl.kielce.tu.neo4j.ogm");

		Session session = sessionFactory.openSession();
		
		session.purgeDatabase();
			
		CinemaService cinemaService = new CinemaService(session);
		TestNeo4JService testNeo4JService = new TestNeo4JService(cinemaService);

		int menu;
		int menu_interrior = -1;

		while(true) {
			System.out.print("Główne Menu:\n");
			System.out.print("\n");
			System.out.print("1.Dodawanie do bazy danych.\n");
			System.out.print("2.Usuwanie całej bazy danych.\n");
			System.out.print("3.Usuwanie rekordu z bazy danych o podanym id.\n");
			System.out.print("4.Wyświetlenie całej bazy danych.\n");
			System.out.print("5.Wyświetlenie rekordu z bazy danych o podanym id.\n");
			System.out.print("6.Wyświetlenie rekordu z bazy danych o podanej nazwie.\n");
			System.out.print("7.Modyfikacja rekordu o podanym id.\n");
			System.out.print("0.Wyjście z programu.\n");

			System.out.print("Wybierz funkcje. I kliknij klawisz ENTER: ");
			menu = scan.nextInt();

			if (menu == 1) {
				System.out.print("Menu dodawania do bazy danych:\n");

				testNeo4JService.addCinema();
				System.out.print("\n");

				if (menu_interrior == 0) {
					System.out.print("\n");
					System.out.print("\n");
					menu_interrior = -1;
					continue;
				}
			}

			if (menu == 2) {

				testNeo4JService.deleteAll();
				System.out.print("Usunięto wszystkie dane z bazy danych.\n");
				System.out.print("\n");
			}

			if (menu == 3) {
				System.out.print("Wyświetlanie wszystkich danych:\n");
				testNeo4JService.ShowAll();
				System.out.print("\n");

				testNeo4JService.deleteCinemaById();
				System.out.print("\n");
			}


			if (menu == 4) {
				System.out.print("\n");
				System.out.print("Wyświetlanie wszystkich danych:\n");
				testNeo4JService.ShowAll();
				System.out.print("\n");
			}

			if (menu == 5) {
				System.out.print("\n");

				System.out.print("Wyświetlanie danych o podanym id:\n");
				testNeo4JService.ShowCinemaById();
				System.out.print("\n");
			}

			if (menu == 6) {
				System.out.print("\n");

				System.out.print("Wyświetlanie kina o podanej nazwie:\n");
				testNeo4JService.ShowCinemaByName();
				System.out.print("\n");
			}

			if (menu == 7) {
				testNeo4JService.ShowAll();
				System.out.print("\n");


				testNeo4JService.updateCinema();
				System.out.print("\n");
			}

			if (menu == 0) {
				System.out.print("\n");
				System.out.print("Wyłączanie programu...\n");
				scan.close();
				sessionFactory.close();
				System.exit(0);
			}


			if (menu < 0 || menu > 7) {
				System.out.print("\n");
				System.out.print("Nie ma takiej opcji!\n");
				System.out.print("\n");
				continue;
			}
		}
	}
}

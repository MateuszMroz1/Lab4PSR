package pl.kielce.tu.neo4j.ogm;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Map;
import java.util.Scanner;

public class TestNeo4JService {
    private CinemaService cinemaService;
    BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
    Scanner scan = new Scanner(System.in);

    public TestNeo4JService(CinemaService cinemaService) {
        this.cinemaService = cinemaService;
    }

    public void addCinema() throws IOException {
        System.out.println("Podaj nazwe filmu: ");

        String nameMovie = in.readLine();

        System.out.println("Podaj cene: ");
        int price = scan.nextInt();

        Movie movie = new Movie(nameMovie, price);

        System.out.println("Podaj nazwe kina: ");
        String cinemaName = in.readLine();

        Cinema cinema1 = new Cinema(cinemaName);
        cinema1.addMovie(movie);

        cinemaService.createOrUpdate(cinema1);
    }

    public void ShowAll(){
        for(Cinema c: cinemaService.readAll())
            System.out.println(c);

        for(Map<String, Object> map : cinemaService.getCinemaRelationships())
            System.out.println(map);
    }

    public void ShowCinemaById(){
        System.out.print("Podaj id kina do wyszukania:\n");
        Long key = scan.nextLong();

        for (Cinema c : cinemaService.readAll())
            if (c.getId().equals(key))
                System.out.println(c);
    }

    public void ShowCinemaByIdArgument(Long key){
        for (Cinema c : cinemaService.readAll())
            if (c.getId().equals(key))
                System.out.println(c);
    }

    public void ShowCinemaByName() throws IOException {
        System.out.print("Podaj nazwe kina do wyszukania:\n");
        String nam = in.readLine();

        for (Cinema c : cinemaService.readAll())
            if (c.getName().equals(nam))
                System.out.println(c);
    }

    public void deleteCinemaById() {
        System.out.print("Podaj klucz do usunięcia:\n");
        Long key = scan.nextLong();
        cinemaService.delete(key);
    }

    public void deleteAll(){
        cinemaService.deleteAll();
    }

    public void updateCinema() throws IOException {
        System.out.println("Podaj id kina, które chcesz zaktualizować");

        Long id = scan.nextLong();

        System.out.println("Podaj nazwe filmu: ");

        String nameMovie = in.readLine();

        System.out.println("Podaj cene: ");
        int price = scan.nextInt();

        System.out.println("Podaj nazwe kina: ");
        String cinemaName = in.readLine();

        Movie movie = new Movie(nameMovie, price);

        Cinema cinema = new Cinema(id, cinemaName);
        cinema.addMovie(movie);

        cinemaService.createOrUpdate(cinema);

        System.out.println("\n");
        System.out.println("Zmienione kino");
        ShowCinemaByIdArgument(id);
    }

}

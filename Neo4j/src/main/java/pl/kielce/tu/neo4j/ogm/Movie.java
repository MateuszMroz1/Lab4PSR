package pl.kielce.tu.neo4j.ogm;

import org.neo4j.ogm.annotation.GeneratedValue;
import org.neo4j.ogm.annotation.Id;
import org.neo4j.ogm.annotation.NodeEntity;
import org.neo4j.ogm.annotation.Property;

@NodeEntity(label = "Movie")
public class Movie{

	@Id
	@GeneratedValue
	private Long id;

	public Long getId() {
		return id;
	}

	public void setId(Long id) {
		this.id = id;
	}

	public String getMovieName() {
		return movieName;
	}

	public void setMovieName(String movieName) {
		this.movieName = movieName;
	}

	public int getPrice() {
		return price;
	}

	public void setPrice(int price) {
		this.price = price;
	}

	public Movie(String movieName, int price) {
		this.movieName = movieName;
		this.price = price;
	}

	@Property(name = "movieName")
	private String movieName;

	@Property(name = "price")
	private int price;

	@Override
	public String toString() {
		return "Odtwarza film = " + movieName + " o cenie = " + price + " zł.";
	}
}
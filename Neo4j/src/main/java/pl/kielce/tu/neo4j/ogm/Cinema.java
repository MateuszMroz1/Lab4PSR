package pl.kielce.tu.neo4j.ogm;

import java.util.HashSet;
import java.util.Set;

import org.neo4j.ogm.annotation.GeneratedValue;
import org.neo4j.ogm.annotation.Id;
import org.neo4j.ogm.annotation.NodeEntity;
import org.neo4j.ogm.annotation.Property;
import org.neo4j.ogm.annotation.Relationship;

@NodeEntity(label = "Cinema")
public class Cinema {

	@Id
	@GeneratedValue
	private Long id;

	@Property(name = "name")
	private String name;
	
	public Cinema() {
	}

	public Cinema(String name) {
		this.name = name;
	}

	public Cinema(Long id, String name) {
		this.id = id;
		this.name = name;
	}

	public Long getId() {
		return id;
	}

	public String getName() {
		return name;
	}

	@Relationship(type = "CINEMA_MOVIES")
	private Set<Movie> movies = new HashSet<>();
	
	public void addMovie(Movie movie) {
		movies.add(movie);
	}

	@Override
	public String toString() {
		return "Kino o id = " + id + " i nazwie = " + name + ". " + movies;
	}
}
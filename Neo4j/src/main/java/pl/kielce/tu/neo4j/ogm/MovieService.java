package pl.kielce.tu.neo4j.ogm;

import org.neo4j.ogm.session.Session;

class MovieService extends GenericService<Movie> {

    public MovieService(Session session) {
		super(session);
	}
    
	@Override
	Class<Movie> getEntityType() {
		return Movie.class;
	}
    
}
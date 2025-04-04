import { getMovies } from "@/api/client/movies-client";
import { MovieResponseDto } from "@/api/models/MovieResponseDto";
import { ErrorAlert } from "@/components/alerts/ErrorAlert";
import { LoadingIndicator } from "@/components/LoadingIndicator";
import { MoviesGrid } from "@/components/movies/MoviesGrid";
import { useEffect, useState } from "react";

/**
 * Shows all movies
 * @constructor
 */
export function MoviesPage() {
    const [movies, setMovies] = useState<MovieResponseDto[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function loadContent() {
            setError(null);
            setIsLoading(true);
            try {
                const loadedMovies = await getMovies();
                setMovies(loadedMovies);
            } catch (e) {
                setError(e instanceof Error ? e.message : "Unknown error.");
            } finally {
                setIsLoading(false);
            }
        }

        loadContent();
    }, []);

    // Render
    if (isLoading) {
        return <LoadingIndicator />;
    }
    
    return (
        <>
            {error ? <ErrorAlert message={error} /> : null}
            <h1>Movies</h1>
            <MoviesGrid movies={movies} />
        </>
    )
}
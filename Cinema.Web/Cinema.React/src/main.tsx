import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.css';
import '@/index.css';

import { RootLayout } from "@/pages/RootLayout";
import { HomePage } from "@/pages/HomePage";
import { MoviesPage } from "@/pages/movies/MoviesPage";
import { MoviePage } from "@/pages/movies/MoviePage";
import { CreateReservationPage } from "@/pages/screenings/CreateReservationPage";
import { NotFoundPage } from "@/pages/NotFoundPage";


const router = createBrowserRouter([
    {
        element: <RootLayout />,
        children: [
            {
                path: "/",
                element: <HomePage />
            },
            {
                path: "/movies",
                element: <MoviesPage />,
            },
            {
                path: "/movies/:movieId",
                element: <MoviePage />,
            },
            {
                path: "/screenings/:screeningId/create-reservation",
                element: <CreateReservationPage />,
            },
            {
                path: "*",
                element: <NotFoundPage />
            },
        ]
    },
]);

createRoot(document.getElementById('root')!).render(
    <RouterProvider router={router} />
);

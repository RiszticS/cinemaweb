import Container from "react-bootstrap/Container";
import { Header } from "@/components/header/Header";
import { RouterSuccessAlert } from "@/components/alerts/RouterSuccessAlert";
import { Outlet } from "react-router-dom";

/**
 * The root layout with navigation header
 * @constructor
 */
export function RootLayout() {
    return (
        <>
            <Header />
            <Container className="my-4">
                <RouterSuccessAlert />

                {/* Outlet is a component form React Router that renders the currently matched page */}
                <Outlet />
            </Container>
        </>
    );
}
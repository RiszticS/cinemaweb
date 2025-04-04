import { HeaderLink } from "@/components/header/HeaderLink";
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';

export function Header() {
   
    return (
        <Navbar variant="light" bg="light" expand="md">
            <Container>
                <Navbar.Brand>ELTE Cinema</Navbar.Brand>
                <Navbar.Toggle aria-controls="header-navbar-nav" />
                <Navbar.Collapse id="header-navbar-nav">
                    <Nav className="me-auto">
                        <HeaderLink to="/" text="Home"/>
                        <HeaderLink to="/movies" text="Movies"/>
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    );
}
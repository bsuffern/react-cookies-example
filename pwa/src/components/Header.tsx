import { Navbar } from "flowbite-react";

export default function Header() {
    return(
        <Navbar fluid rounded>
            <Navbar.Brand>
                <img src="/src/assets/t-shirt.svg" className="mr-3 h-6 sm:h-9" alt="Fashion brand logo" />
                <span className="self-center whitespace-nowrap text-xl font-semibold dark:text-white">Fashion Brand</span>
            </Navbar.Brand>
            <Navbar.Toggle />
            <Navbar.Collapse>
                <Navbar.Link href="#" active>
                    Home
                </Navbar.Link>
                <span>
                    <Navbar.Link href="#">Cart</Navbar.Link>
                    <img src="/src/assets/shopping-cart.svg" className="mr-3 h-6 sm:h-9" alt="Shopping cart icon" />
                </span>
            </Navbar.Collapse>
        </Navbar>
    );
}
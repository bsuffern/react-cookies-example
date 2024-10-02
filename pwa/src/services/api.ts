import Cart from "@/models/Cart";

const url = import.meta.env.VITE_API_ENDPOINT_URL;

export async function GetCart(cartId:string) {
    try {
        const response = await fetch(url + `/api/Carts/${cartId}`);

        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const data = response.body;

        console.log(data);

    } catch (error:any) {
        console.log(error.message);
    }
}

export async function CreateCart() {

}

export async function GetProducts() {

}

export async function DeleteProduct() {

}
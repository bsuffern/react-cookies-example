import Product from "@/components/Product";
import Layout from "@/components/Layout";

export default function ProductsPage() {
    return(
        <Layout>
            <Product name="Belt" desc="Brown leather belt" imgSrc="\src\assets\example-belt.png"></Product>
            <Product name="Dress" desc="Navy dress" imgSrc="\src\assets\example-dress.png"></Product>
        </Layout>
    );
}

namespace Catalog.API.Products.DeleteProduct
{
    //public record DeleteProductRequest(Guid Id);
    public record DeleteProductResponse(bool IsSuccess);
    public class DeleteProductEndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/product/{id}", async(Guid id,ISender sender)=>
            {

                // Create a command for deleting the product
                var command = new DeleteProductCommand(id);

                // Send the command to the MediatR pipeline
                var result = await sender.Send(command);
                var response = result.Adapt<DeleteProductResponse>();

                // Return an appropriate response based on the result
                return Results.Ok(response);
            })
            .WithName("DeleteProduct") // Sets a name for the route
            .WithDescription("Deletes a product by its unique identifier.") // Adds a description
            .ProducesProblem(StatusCodes.Status400BadRequest) 
            .ProducesProblem(StatusCodes.Status404NotFound) // Indicates a 404 response
            .WithSummary("Delete Product")
            .WithDescription("Delete Product");

        }
    }
}

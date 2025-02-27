using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            //Convert CouponModel to Coupon
           var coupon = request.Coupon.Adapt<Coupon>();

            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

            dbContext.Add(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Discount is successfully created. ProductName={ProductName}", coupon.ProductName);

            //Convert coupon to Coupon Model
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public  override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext
                           .Coupons
                           .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if (coupon is null) { 
                coupon = new Coupon{ ProductName="No Discount", Amount = 0, Description="No Discount Desc" };
            }

            logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount: {amount}",
                coupon.ProductName, coupon.Amount);


            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;

        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            //Convert CouponModel to Coupon
            var coupon = request.Coupon.Adapt<Coupon>();

            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

            dbContext.Update(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Discount is successfully updated. ProductName={ProductName}", coupon.ProductName);

            //Convert coupon to Coupon Model
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbContext
                           .Coupons
                           .FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with product name =  {request.ProductName} is not found"));
            }

            dbContext.Coupons.Remove(coupon);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"Discount is successfully deleted for ProductName : {request.ProductName}",
                coupon.ProductName, coupon.Amount);

            return new DeleteDiscountResponse { Success= true};
        }
    }
}

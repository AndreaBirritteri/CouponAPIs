﻿using System.Net;
using AutoMapper;
using CouponAPI.Filters;
using CouponAPI.Models;
using CouponAPI.Models.DTO;
using CouponAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace CouponAPI.Endpoints;

public static class CouponEndpoints
{
    public static void ConfigureCouponEndpoints(this WebApplication app)
    {
        app.MapGet("/api/coupon", GetAllCoupon)
            .WithName("GetCoupons").Produces<APIResponse>();
        //.RequireAuthorization("AdminOnly") ;

        app.MapGet("/api/coupon/{id:int}", GetCoupon)
            .WithName("GetCoupon").Produces<APIResponse>()
            .AddFilter<ParameterIDValidator>();

        app.MapPost("/api/coupon", CreateCoupon)
            .WithName("CreateCoupon")
            .Accepts<CouponCreateDTO>("application/json")
            .Produces<APIResponse>(201)
            .Produces(400)
            .AddFilter<BasicValidator<CouponCreateDTO>>();

        app.MapPut("/api/coupon", UpdateCoupon)
            .WithName("UpdateCoupon")
            .Accepts<CouponUpdateDTO>("application/json")
            .Produces<APIResponse>().Produces(400)
            .AddFilter<BasicValidator<CouponUpdateDTO>>();

        app.MapDelete("/api/coupon/{id:int}", DeleteCoupon).AddFilter<ParameterIDValidator>();
    }

    private static async Task<IResult> GetCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger, int id)
    {
        Console.WriteLine("Endpoint executed.");
        APIResponse response = new();
        response.Result = await _couponRepo.GetAsync(id);
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    // [Authorize]
    private static async Task<IResult> CreateCoupon(ICouponRepository _couponRepo, IMapper _mapper,
        [FromBody] CouponCreateDTO coupon_C_DTO)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

        if (_couponRepo.GetAsync(coupon_C_DTO.Name).GetAwaiter().GetResult() != null)
        {
            response.ErrorMessages.Add("Coupon Name already Exists");
            return Results.BadRequest(response);
        }

        var coupon = _mapper.Map<Coupon>(coupon_C_DTO);


        await _couponRepo.CreateAsync(coupon);
        await _couponRepo.SaveAsync();
        var couponDTO = _mapper.Map<CouponDTO>(coupon);


        response.Result = couponDTO;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.Created;
        return Results.Ok(response);
        //return Results.CreatedAtRoute("GetCoupon",new { id=coupon.Id }, couponDTO);
        //return Results.Created($"/api/coupon/{coupon.Id}",coupon);
    }

    // [Authorize]
    private static async Task<IResult> UpdateCoupon(ICouponRepository _couponRepo, IMapper _mapper,
        [FromBody] CouponUpdateDTO coupon_U_DTO)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


        await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(coupon_U_DTO));
        await _couponRepo.SaveAsync();

        response.Result = _mapper.Map<CouponDTO>(await _couponRepo.GetAsync(coupon_U_DTO.Id));
        ;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    //  [Authorize]
    private static async Task<IResult> DeleteCoupon(ICouponRepository _couponRepo, int id)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


        var couponFromStore = await _couponRepo.GetAsync(id);
        if (couponFromStore != null)
        {
            await _couponRepo.RemoveAsync(couponFromStore);
            await _couponRepo.SaveAsync();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.NoContent;
            return Results.Ok(response);
        }

        response.ErrorMessages.Add("Invalid Id");
        return Results.BadRequest(response);
    }

    private static async Task<IResult> GetAllCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger)
    {
        APIResponse response = new();
        _logger.Log(LogLevel.Information, "Getting all Coupons");
        response.Result = await _couponRepo.GetAllAsync();
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }
}
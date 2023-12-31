﻿using System.Net;
using CouponAPI.Models;
using CouponAPI.Models.DTO;
using CouponAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace CouponAPI.Endpoints;

public static class AuthEndpoints
{
    public static void ConfigureAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/api/login", Login).WithName("Login").Accepts<LoginRequestDTO>("application/json")
            .Produces<APIResponse>().Produces(400);
        app.MapPost("/api/register", Register).WithName("Register").Accepts<RegisterationRequestDTO>("application/json")
            .Produces<APIResponse>().Produces(400);
    }


    private static async Task<IResult> Register(IAuthRepository _authRepo,
        [FromBody] RegisterationRequestDTO model)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


        var ifUserNameisUnique = _authRepo.IsUniqueUser(model.UserName);
        if (!ifUserNameisUnique)
        {
            response.ErrorMessages.Add("Username already exists");
            return Results.BadRequest(response);
        }

        var registerResponse = await _authRepo.Register(model);
        if (registerResponse == null || string.IsNullOrEmpty(registerResponse.UserName))
            return Results.BadRequest(response);

        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }

    private static async Task<IResult> Login(IAuthRepository _authRepo,
        [FromBody] LoginRequestDTO model)
    {
        APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
        var loginResponse = await _authRepo.Login(model);
        if (loginResponse == null)
        {
            response.ErrorMessages.Add("Username or password is incorrect");
            return Results.BadRequest(response);
        }

        response.Result = loginResponse;
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.OK;
        return Results.Ok(response);
    }
}
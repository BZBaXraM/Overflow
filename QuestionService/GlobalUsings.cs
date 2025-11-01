// Global using directives

global using System.ComponentModel.DataAnnotations;
global using System.Security.Claims;
global using System.Text.Json.Serialization;
global using Contracts;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Memory;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using QuestionService.Data;
global using QuestionService.DTOs;
global using QuestionService.Entities;
global using QuestionService.Services;
global using QuestionService.Validators;
global using Wolverine;
global using Wolverine.RabbitMQ;
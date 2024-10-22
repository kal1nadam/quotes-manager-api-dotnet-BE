using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quotesApi.DTOs.Quotes;
using quotesApi.Enums;
using quotesApi.Models;
using quotesApi.ORM;
using quotesApi.ORM.Entities;

namespace quotesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class QuotesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    
    public QuotesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    // GET: api/quotes/random
    [HttpGet("random")]
    public async Task<ActionResult<QuoteModel>> GetRandomQuote()
    {
        var quote = await _dbContext.Quotes
            .Select(q => new QuoteModel
            {
                Id = q.Id,
                Text = q.Text,
                Author = q.Author,
                Tags = q.Tags.Select(t => t.Type).ToList()
            })
            .OrderBy(q => Guid.NewGuid())
            .FirstOrDefaultAsync();

        return Ok(quote);
    }
    
    
    // GET: api/quotes
    [HttpGet]
    public async Task<ActionResult<GetQuotesResponse>> GetAllQuotes()
    {
        var quotes = await _dbContext.Quotes
            .Select(q => new QuoteModel { 
                Id = q.Id,
                Text = q.Text, 
                Author = q.Author, 
                Tags = q.Tags.Select(t => t.Type).ToList()
            }).ToListAsync();

        GetQuotesResponse response = new()
        {
            Quotes = quotes
        };
        
        return Ok(response);
    }
    
    // get filtered quotes by multiple tags
    // GET: api/quotes/tags?tags=tag1,tag2
    [HttpGet("tags")]
    public async Task<ActionResult<GetQuotesResponse>> GetQuotesByTags([FromQuery] string tags)
    {
        var tagsList = tags.Split(',').Select(Enum.Parse<QuoteTagType>).ToList();
        
        var quotes = await _dbContext.Quotes
            .Where(q => q.Tags.Any(t => tagsList.Contains(t.Type)))
            .Select(q => new QuoteModel
            {
                Id = q.Id,
                Text = q.Text, 
                Author = q.Author,
                Tags = q.Tags.Select(t => t.Type).ToList()
            }).ToListAsync();
        
        GetQuotesResponse response = new()
        {
            Quotes = quotes
        };
        
        return Ok(response);
    }

    // GET: api/quotes/{userId}
    [HttpGet("{userId}")]
    public async Task<ActionResult<GetQuotesResponse>> GetQuotesByUser(Guid userId)
    {
        var quotes = await _dbContext.Quotes
            .Where(q => q.UserId == userId)
            .Select(q => new QuoteModel
            {
                Id = q.Id,
                Text = q.Text, 
                Author = q.Author,
                Tags = q.Tags.Select(t => t.Type).ToList()
            }).ToListAsync();
        
        GetQuotesResponse response = new()
        {
            Quotes = quotes
        };
        
        return Ok(response);
    }
    
    // get filtered quotes by multiple tags and userId
    // GET: api/quotes/{userId}/tags?tags=tag1,tag2
    [HttpGet("{userId}/tags")]
    public async Task<ActionResult<GetQuotesResponse>> GetQuotesByUserAndTags(Guid userId, [FromQuery] string tags)
    {
        var tagsList = tags.Split(',').Select(Enum.Parse<QuoteTagType>).ToList();
        
        var quotes = await _dbContext.Quotes
            .Where(q => q.UserId == userId && q.Tags.Any(t => tagsList.Contains(t.Type)))
            .Select(q => new QuoteModel
            {
                Id = q.Id,
                Text = q.Text, 
                Author = q.Author,
                Tags = q.Tags.Select(t => t.Type).ToList()
            }).ToListAsync();
        
        GetQuotesResponse response = new()
        {
            Quotes = quotes
        };
        
        return Ok(response);
    }
    

    // DELETE: api/quotes/{quoteId}
    [HttpDelete("{quoteId}")]
    [Authorize] // Require authorization
    public async Task<IActionResult> DeleteQuote(Guid quoteId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
        if(userId == null)
        {
            return Unauthorized();
        }

        var quote = await _dbContext.Quotes.FindAsync(quoteId);
        if (quote == null)
        {
            return NotFound("Quote not found.");
        }

        // Only allow admin or the owner of the quote to delete it
        var isAdmin = User.IsInRole("admin");
        if (quote.UserId != Guid.Parse(userId) && !isAdmin)
        {
            return Forbid("You do not have permission to delete this quote.");
        }

        _dbContext.Quotes.Remove(quote);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
    
    // PUT api/quotes/{quoteId}
    [HttpPut("{quoteId}")]
    [Authorize]
    public async Task<IActionResult> UpdateQuote(Guid quoteId, UpdateQuoteRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
        {
            return Unauthorized();
        }

        var quote = await _dbContext.Quotes.Include(q => q.Tags).FirstOrDefaultAsync(q => q.Id == quoteId);
        if (quote == null)
        {
            return NotFound("Quote not found.");
        }

        // Only allow admin or the owner of the quote to update it
        var isAdmin = User.IsInRole("admin");
        if (quote.UserId != Guid.Parse(userId) && !isAdmin)
        {
            return Forbid("You do not have permission to update this quote.");
        }

        quote.Text = request.Text;
        quote.Author = request.Author;
        
        // Update tags
        // Create new records
        var newTags = request.Tags.Select(t => new QuoteTag { Type = t }).ToList();
        quote.Tags = newTags;

        _dbContext.Update(quote);

        return Ok();
    }
    
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreateQuoteResponse>> CreateQuote(CreateQuoteRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId == null)
        {
            return Unauthorized();
        }

        var quote = new Quote
        {
            Text = request.Text,
            Author = request.Author,
            UserId = Guid.Parse(userId),
            Tags = request.Tags.Select(t => new QuoteTag(){Type = t}).ToList()
        };

        _dbContext.Quotes.Add(quote);
        await _dbContext.SaveChangesAsync();
        
        QuoteModel quoteCreated = new()
        {
            Id = quote.Id,
            Text = quote.Text,
            Author = quote.Author,
            Tags = quote.Tags.Select(t => t.Type).ToList()
        };
        
        CreateQuoteResponse response = new()
        {
            Quote = quoteCreated
        };

        return Ok(response);
    }
}
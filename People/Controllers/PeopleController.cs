using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using People.Models;

namespace People.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private static List<Person> _people = new List<Person>();
        private readonly AppDbContext _context;

        public PeopleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            return Ok(await _context.People.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(Guid id)
        {
            var person =  await _context.People
                .Include(x => x.ContactInfos)
                .FirstOrDefaultAsync( x => x.Id == id);
            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto person)
        {
            var newPerson = new Person()
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Company = person.Company
            };
            var contacts = new List<ContactInfo>();
            person.ContactInfos.ForEach(x =>
            {
                contacts.Add(new ContactInfo()
                {
                    InfoType = x.InfoType,
                    InfoContent = x.InfoContent
                });
            });
            newPerson.ContactInfos.AddRange(contacts);
            await _context.AddAsync(newPerson); 
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetPerson), new { id = newPerson.Id }, new { id = newPerson.Id });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
                return NotFound();

            _context.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/contact")]
        public async Task<IActionResult> AddContact(Guid id, [FromBody] ContactInfoDto contact)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
                return NotFound();

            await _context.ContactInfos.AddAsync(new ContactInfo {
                InfoContent = contact.InfoContent,
                InfoType = contact.InfoType,
                PersonId = person.Id
            });
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/contact/{contactId}")]
        public async Task<IActionResult> RemoveContact(Guid id, Guid contactId)
        {
            var person = await _context.People
                .Include(x => x.ContactInfos)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (person == null)
                return NotFound();

            var contact =  person.ContactInfos.FirstOrDefault(x => x.Id == contactId);
            if (contact == null)
                return NotFound();

            person.ContactInfos.Remove(contact);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
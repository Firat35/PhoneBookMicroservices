using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using People.Models;
using People.Repository;

namespace People.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        //private static List<Person> _people = new List<Person>();
        //private readonly AppDbContext _context;
        private readonly IRepository<Person> _repoPerson;
        private readonly IRepository<ContactInfo> _repoContact;

        public PeopleController(
            IRepository<Person> repoPerson,
            IRepository<ContactInfo> repoContact)
        {
            _repoPerson = repoPerson;
            _repoContact = repoContact;
        }

        [HttpGet]
        public async Task<IActionResult> GetPeople()
        {
            return Ok(await _repoPerson.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(Guid id)
        {
            var person = await _repoPerson.GetByIdAsync(id);
            if (person == null)
                return NotFound();
            person.ContactInfos = await _repoContact.Where(x => x.PersonId== id).ToListAsync();
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

            await _repoPerson.CreateAsync(newPerson);

            return CreatedAtAction(nameof(GetPerson), new { id = newPerson.Id }, new { id = newPerson.Id });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            var person = await _repoPerson.GetByIdAsync(id);
            if (person == null)
                return NotFound();

            await _repoPerson.DeleteAsync(person);

            return NoContent();
        }

        [HttpPost("{id}/contact")]
        public async Task<IActionResult> AddContact(Guid id, [FromBody] ContactInfoDto contact)
        {
            var person = await _repoPerson.GetByIdAsync(id);
            if (person == null)
                return NotFound();

            await _repoContact.CreateAsync(new ContactInfo {
                InfoContent = contact.InfoContent,
                InfoType = contact.InfoType,
                PersonId = person.Id
            });

            return NoContent();
        }

        [HttpDelete("{id}/contact/{contactId}")]
        public async Task<IActionResult> RemoveContact(Guid id, Guid contactId)
        {
            var person = await _repoPerson.GetByIdAsync(id);
            if (person == null)
                return NotFound("Person");

            var contact = await _repoContact.GetByIdAsync(contactId);
            if (contact == null)
                return NotFound("Contact");

            await _repoContact.DeleteAsync(contact);

            return NoContent();
        }
    }
}
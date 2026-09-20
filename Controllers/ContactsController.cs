using ContactApp.Models;
using ContactApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactApp.Controllers
{
    public class ContactsController : Controller
    {

        private readonly IContactRepository _repo;
        private readonly ILogger<ContactsController> _logger;
        public ContactsController(IContactRepository repo, ILogger<ContactsController> logger)
        {
            this._repo = repo;
            this._logger = logger;
        }
        [HttpGet]
        public IActionResult Index(string? q)
        {
            var items = _repo.GetAll();
            if(q==null)
            {
                ViewData["Title"] = "All Contacts";
            }
            else
            {
                q = q.Trim();
                ViewData["Title"] = $"Search results for '{q}'";
                ViewBag.Query = q;
                items = items.Where(c => (c.FirstName+" "+c.LastName).Contains(q, StringComparison.CurrentCultureIgnoreCase) || (c.FirstName).Contains(q, StringComparison.CurrentCultureIgnoreCase)||(c.LastName).Contains(q,StringComparison.CurrentCultureIgnoreCase));//her contact 'ın ad ve soyadını birleştirip, q parametresini içerip içermediğini kontrol ediyoruz. Contains metodu büyüklük küçüklüğü göz ardı edecek şekilde ayarlandı.
            }
            return View(items.ToList());
        }
        public IActionResult Details(int id)
        {
            var contact = _repo.GetById(id);
            if (contact == null)
            {
                return NotFoundView();
            }
            ViewData["Title"] = "Update Contact";
            return View(contact);
        }

        private IActionResult NotFoundView()
        {
            Response.StatusCode = 404;
            ViewData["Title"] = "Contact Not Found";
            return View("NotFound");

        }
        [HttpGet("create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Create New Contact";
            return View(new Contact());//boş ama geçerli bir Contact nesnesi göndererek view'a "işte doldurman gereken şablon bu" demiş oluyoruz.
        }
        // Bu action sadece POST isteklerine cevap verir, route "create" olarak sabitlenmiş (örn: /Contact/create)
        [HttpPost("create")]
        public IActionResult Create(Contact model)
        {
            // model parametresi, framework tarafından form verilerinden otomatik dolduruldu (model binding)
            // bu satıra gelmeden ÖNCE ModelState de otomatik olarak hesaplandı (validation attribute'lara göre)

            // Eğer model üzerindeki [Required], [StringLength] vs. kurallarından biri bile ihlal edildiyse buraya girer
            if (!ModelState.IsValid)
            {
                // Sayfa başlığını tekrar ayarlıyoruz çünkü view yeniden render edilecek
                ViewData["Title"] = "Create New Contact";

                // Kullanıcının girdiği (hatalı da olsa) verilerle formu tekrar gösteriyoruz
                // böylece kullanıcı doğru girdiği alanları yeniden yazmak zorunda kalmıyor
                return View(model);
            }

            // Validasyon başarılıysa, yeni contact'ı repository/veritabanına ekliyoruz
            _repo.Add(model);

            // Yapılandırılmış (structured) log kaydı düşüyoruz
            // {FirstName} ve {LastName} placeholder'ları model.FirstName / model.LastName ile değiştirilir
            _logger.LogInformation("New contact created: {FirstName} {LastName}", model.FirstName, model.LastName);

            // Bir sonraki (redirect sonrası) isteğe taşınacak başarı mesajını TempData'ya koyuyoruz
            // TempData, normal ViewData'nın aksine bir sonraki HTTP isteğine kadar hayatta kalır
            TempData["SuccessMessage"] = "Contact created successfully!";

            // Post-Redirect-Get pattern: tarayıcıya 302 döndürüp Index action'ına yönlendiriyoruz
            // böylece kullanıcı F5'e basarsa form tekrar POST edilmez, tekrar kayıt oluşmaz
            return RedirectToAction("Index");
        }

        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var contact = _repo.GetById(id);
            if (contact == null)
            {
                return NotFoundView();
            }
            ViewData["Title"] = "Edit Contact";
            return View(contact);
        }
        [HttpPost("edit/{id}")]
        public IActionResult Edit(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFoundView();
            }
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Edit Contact";
                return View(contact);
            }
            else
            {
                var existingContact = _repo.Update(contact);
                if (!existingContact)
                {
                    return NotFoundView();
                }
                _logger.LogInformation("Contact updated: {FirstName} {LastName}", contact.FirstName, contact.LastName);
                TempData["SuccessMessage"] = "Contact updated successfully!";
                return RedirectToAction("Index");
            }
        }
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {



            var contact = _repo.GetById(id);
            if (contact == null)
            {
                return NotFoundView();
            }
            ViewData["Title"] = "Delete Contact";
            return View(contact);
        }
        [HttpPost("delete/{id}")]//model binding ile id parametresini alabilmek için route parametresi olarak {id} ekledik.
        [ActionName("Delete")]// Bu action'ın adı Delete olmasına rağmen, route "delete/{id}" ile eşleşir. Asıl mühim olan ise function overloading sıkıntısı giderilmiş olur.
        public IActionResult DeleteConfirmed(int id)
        {

            var status = _repo.Delete(id);
            if(!status)
            {
                return NotFoundView();
            }
            TempData["Success"] = "Contact deleted successfully!"; 
            return RedirectToAction("Index");


        }
    }
}

[TestFixture]
[Author("Steven Mead", "steven.j.mead@tees.ac.uk")]
public class AmericanInvoicingTest {    
    private string? expectedContent;

    [SetUp]
    public void SetUp()
    {
        expectedContent = InvoiceFileUtility.ReadFile("../../../../data/expected-american.txt", TestContext.Out);
    }

    [TestCase("Oracle", 500)]
    public void GetInvoiceTest(string customerName, int amount) {
        var customer = new Customer(customerName, amount, new AmericanInvoice());

        customer.ChangeCurrency(new AmericanInvoice());

        Console.WriteLine(customer.Name);
        Console.WriteLine(customer.GetInvoice());

        //Assert.That(customer.GetInvoice(), Is.EqualTo(expectedContent));
    }
}

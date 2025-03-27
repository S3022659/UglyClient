/*
[TestFixture]
[Author("Steven Mead", "steven.j.mead@tees.ac.uk")]
public class EuropeanInvoicingTest {    
    private string? expectedContent;

    [SetUp]
    public void SetUp()
    {
        expectedContent = InvoiceFileUtility.ReadFile("../../../../data/expected-european.txt", TestContext.Out);
    }

    [TestCase("Siemens", 17023)]
    public void GetInvoiceTest(string customerName, double amount) {
        var customer = new Customer(customerName, amount);

        customer.InvoiceAlgorithm = new EuropeanInvoice();

        Assert.That(customer.GetInvoice(), Is.EqualTo(expectedContent));
    }
}
*/
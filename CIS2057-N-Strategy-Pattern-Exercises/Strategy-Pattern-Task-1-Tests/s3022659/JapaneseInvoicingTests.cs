/*
[TestFixture]
[Author("Steven Mead", "steven.j.mead@tees.ac.uk")]
public class JapaneseInvoicingTest {

    private string? expectedContent;

    [SetUp]
    public void SetUp()
    {
        expectedContent = InvoiceFileUtility.ReadFile("../../../../data/expected-japanese.txt", TestContext.Out);
    }

    [TestCase("Honda", 7200)]
    public void GetInvoiceTest(string customerName, double amount) {
        var customer = new Customer(customerName, amount)
        {
            InvoiceAlgorithm = new JapaneseInvoice()
        };

        Assert.That(customer.GetInvoice(), Is.EqualTo(expectedContent));
    }
}
*/
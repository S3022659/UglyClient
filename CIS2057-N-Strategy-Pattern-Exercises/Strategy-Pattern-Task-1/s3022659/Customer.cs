using System;

public interface IInvoicingAlgo
{
    double Convert(double amount);

Invoice getInvoice(string name, double amount);
}

public class AmericanInvoice : IInvoicingAlgo
{
    public double Convert(double amount)
    {
        return amount * 1.57;
    }

    public Invoice getInvoice(string name, double amount)
    {
        throw new NotImplementedException();
    }
}

public class EuropeanInvoice : IInvoicingAlgo
{
    public double Convert(double amount)
    {
        return amount * 1.14;
    }

    public Invoice getInvoice(string name, double amount)
    {
        throw new NotImplementedException();
    }
}

public class JapaneseInvoice : IInvoicingAlgo
{
    public double Convert(double amount)
    {
        return amount * 121;
    }

    public Invoice getInvoice(string name, double amount)
    {
        throw new NotImplementedException();
    }
}

public class Invoice
{
    public double Amount { get; set; }
    private IInvoicingAlgo _currencyStrategy;

    public Invoice(double amount, IInvoicingAlgo strategy)
    {
        Amount = amount;
        _currencyStrategy = strategy;
    }

    public double GetConvertedAmount()
    {
        return _currencyStrategy.Convert(Amount);
    }

    public IInvoicingAlgo GetCurrencyStrategy()
    {
        return _currencyStrategy;
    }

    public void InvoicingAlgorithm(IInvoicingAlgo strategy)
    {
        _currencyStrategy = strategy;
    }
}

public class Customer(string name, double amount, IInvoicingAlgo strategy)
{
    public string Name { get; set; } = name;
    public double Amount { get; set; } = amount;
    private Invoice _invoice = new Invoice(amount, strategy);

    public Invoice Invoice
    {
        get { return GetInvoice(); }
        set { _invoice = value; }
    }

    public Invoice GetInvoice()
    {
        return _invoice;
    }

    public void ChangeCurrency(IInvoicingAlgo newStrategy)
    {
        _invoice.InvoicingAlgorithm(newStrategy);
    }
}
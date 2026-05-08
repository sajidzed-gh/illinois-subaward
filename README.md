# SubawardReader

A .NET 10 console application that reads budget spreadsheets and extracts subaward information.

## Usage

Navigate to the solution folder:

```bash
cd C:\path\to\SubawardReader
```

Run:

```bash
dotnet run --project SubawardReader.App -- "C:\path\to\your\spreadsheets"
```

---

## Assumptions

### Spreadsheet Structure
- All data is on the **first worksheet** of each file
- Files must be in `.xlsx` format — older `.xls` files are not supported
- All Excel files must sit **directly in the specified folder** — subfolders are not scanned

### Subaward Row Identification
- A subaward row is identified by **column B starting with `"Subaward:"`** (case insensitive)
- The recipient name is either:
  - Embedded in column B after the colon e.g. `"Subaward: Indiana"`
  - Or in column C when column B only contains `"Subaward:"`

### Exempt Row
- The row **immediately following** each subaward row is expected to be the `"Exempt Subaward Costs (>$25k)"` row
- Both amounts are **added together** to calculate the true total paid to that recipient
- This reflects the fact that the exempt portion is the same subaward payment split purely for overhead/indirect cost accounting purposes

### Total Amount Column
- The code reads the **last numeric column** in each subaward row as the total amount
- If no `"Total"` header is found it falls back to the last numeric column in the row

### Recipient Naming
- Recipient names are treated as **case insensitive** when grouping across files
- If no name can be found the recipient is labeled `"(Unknown)"`

---

## Background(Assumptions — To Be Confirmed)

### What is a Subaward?
A subaward is money passed from the main grant recipient (e.g. University of Michigan) down to a partner organization (e.g. Indiana University) to complete a specific part of the research project.

```
NIH (Funding Agency)
    ↓  awards grant to
Main University (e.g. Michigan)
    ↓  subawards portion to
Partner University (e.g. Indiana)
```

### Why are there two rows per subaward?
NIH rules cap the amount of overhead/indirect costs the main university can charge on subawards at **$25,000**.

| Row | Amount | Overhead Charged? |
|---|---|---|
| `Subaward: Indiana` | $25,000 | Yes |
| `Exempt Subaward Costs (>$25k)` | $101,904 | No |
| **Total paid to Indiana** | **$126,904** | |

The split is purely for internal accounting — Indiana receives the full amount as one payment.

---

## Project Structure

```
SubawardReader/
├── SubawardReader.sln
├── SubawardBusiness.Core/          # Parsing library
│   ├── SubawardRecord.cs         # Data model
│   └── SubawardExcelParser.cs    # ClosedXML-based parser
├── SubawardReader.App/           # Console entry point
│   └── Program.cs
└── SubawardReader.Tests/         # xUnit test project
    ├── UnitTest1.cs
    └── TestData/                 # Sample spreadsheets
```

---

## Dependencies
- [ClosedXML](https://github.com/ClosedXML/ClosedXML) — for reading `.xlsx` files
- [xUnit](https://xunit.net/) — for unit testing

---

## 🤖 Alternative Approach — AI Based Parsing
> 💡 **Future Enhancement?** This entire parsing logic could be replaced with a single AI prompt — automatically adapting to any spreadsheet layout without code changes.

# unity-csv-asset

This plugin provides a scriptable object to read CSV files in the project in a structured manner.

There are many CSV readers available online. This one correctly parses them according to RFC 4180.

## Installation

Copy the following snippet into `dependencies` in your project `manifest.json` file.
```
"com.framepush.unity-csv-asset": "https://github.com/FramePush/unity-csv-asset.git",
```
Your file should end up looking something like this.
```json5
{
  "dependencies": {
    "com.framepush.unity-csv-asset": "https://github.com/FramePush/unity-csv-asset.git",
    "com.unity.cinemachine": "2.8.9",
    "com.unity.ide.rider": "3.0.18",
    "com.unity.ide.visualstudio": "2.0.17",
    "com.unity.ide.vscode": "1.2.5"
    // Shortened for brevity
  }
}
```

## Usage

This plugin adds a new asset type to the Create Asset menu called 'CsvFile'.

1. Add your csv file to your assets folder. The Unity Editor will import it as a text asset.
2. Create a CsvFile asset from the Create Asset menu.
3. Set the `source` field on the CsvFile to the text asset from step 1.
4. Set whether the file has headers using the check box.
5. Get a reference to the CsvFile asset in your code.
6. Call the `Parse` method to extract the text into structured data.
7. Use the `Records` field to access data.

```csharp
using FramePush.Csv;

public class Example : MonoBehaviour
{
    public CsvFile csv;
    
    private void Awake()
    {
        // This must be called before data becomes available
        csv.Parse();
    }
    
    private void OnEnable()
    {
        // Get a single record
        Record firstRecord = csv.Records[0];
        // Get a single field by index
        string firstField = firstRecord[0];
        // Get a single field by header
        string anotherField = firstRecord["Some Header"];
        // You can also access headers when present
        string[] headers = csv.Headers;
    }
}
```
You can also use the parser directly without creating assets.
```csharp
var (records, headers) = CsvFile.Parse(text, hasHeaders);
// or
var records = CsvFile.Parse(text);
```

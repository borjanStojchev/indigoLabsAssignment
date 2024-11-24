# **Indigo Labs Assignment**

This project is a .NET 8 Web API that calculates and retrieves average temperatures for cities based on data from a CSV file. It includes features such as API key-based authentication, automatic recalculation on file changes, and Swagger documentation.

## **Features**

1. **Calculates Average Temperature of Each City**
   - Caches it so that it does not need to recalculate each time

2. **Endpoints**:
   - **GET /api/temperature**: Retrieve average temperatures for all cities.
   - **GET /api/temperature/{cityName}**: Retrieve the average temperature for a specific city.
   - **GET /api/temperature/filter**: Retrieve a list of cities filtered by temperature (e.g., larger or smaller than a specified value).

3. **CSV Parsing**:
   - The system uses `Sep` for parsing the `measurements.txt` CSV file, where data is delimited by semicolons (`;`).


4. **API Key Authentication**:
   - Two valid API keys are supported:
     - `key1-123456`
     - `key2-abcdef`
   - Authentication is required for all endpoints. Provide the API key in the `Authorization` header in the format:

     `Authorization: ApiKey <your-api-key>`

5. **Automatic Recalculation**:
   - If the `measurements.txt` file is replaced or modified, the system automatically recalculates city averages using a `FileSystemWatcher`.


6. **Swagger Documentation**:
   - Swagger UI is available for local testing and exploration of the API.
   - Access Swagger at: [http://localhost:5212/swagger/index.html](http://localhost:5212/swagger/index.html)

---
Development Details
===================

**Data**
- A reduced version of `measurements.txt` is included in the repository.
- To use the original file, place it in the project directory in `Data` folder.

**File Monitoring:**
- The application uses FileSystemWatcher to monitor changes to measurements.txt.
- If the file is modified, renamed, or replaced, city averages are automatically recalculated and cached.

**Authentication:**
- API Key-based authentication is implemented.
- The Authorization header must contain a valid API key in the format:
  ApiKey <your-api-key>
- Unauthorized requests will return a 401 Unauthorized response.

**Swagger:**
- Swagger UI is configured for API testing and can be accessed locally at:
  http://localhost:5212/swagger/index.html
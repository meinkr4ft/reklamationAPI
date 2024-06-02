import requests
import urllib3

# disable warnings when no certificate is used 
urllib3.disable_warnings()


# base url for api calls
base_url = "https://localhost:7069/api"

# header for GET
headers = {
    "accept": "text/plain",
    "Content-Type": "application/json"
}

# login data
invalidUser = {
  "username": "invalidUser",
  "password": "InvalidUser!123"
}
loginUser = {
  "username": "user",
  "password": "User!123"
}

loginAdmin = {
  "username": "admin",
  "password": "Admin!123"
}

# sample data
complaint1 = {
  "productId": 101,
  "customer": {
    "email": "john.doe@example.com",
    "name": "John Doe"
  },
  "date": "2023-03-25",
  "description": "Das Produkt funktioniert nicht wie erwartet.",
  "status": "Open"
}

complaint2 = {
    "productId": 202,
    "customer": {
        "email": "jane.smith@example.com",
        "name": "Jane Smith"
    },
    "date": "2023-04-10",
    "description": "Produkt kam beschädigt an.",
    "status": "Open"
}

complaint3 = {
    "productId": 101,
    "customer": {
        "email": "bob.jones@example.com",
        "name": "Bob Jones"
    },
    "date": "2023-05-20",
    "description": "Falsche Größe geliefert.",
    "status": "InProgress"
}

complaint4 = {
    "productId": 303,
    "customer": {
        "email": "mary.johnson@example.com",
        "name": "Mary Johnson"
    },
    "date": "2023-06-15",
    "description": "Produkt entspricht nicht der Beschreibung.",
    "status": "Accepted"
}

complaint5 = {
    "productId": 202,
    "customer": {
        "email": "alice.jones@example.com",
        "name": "Alice Jones"
    },
    "date": "2023-06-01",
    "description": "Defektes Produkt.",
    "status": "InProgress"
}

complaint6 = {
    "productId": 101,
    "customer": {
        "email": "alex.gray@example.com",
        "name": "Alex Gray"
    },
    "date": "2024-04-15",
    "description": "Fehlende Teile.",
    "status": "Accepted"
}

# api calls
def login(data = ""):
    url = f"{base_url}/Auth/login"
    return requests.post(url, json=data, headers=headers,verify=False)

def get_complaints(id = ""):
    url = f"{base_url}/complaints/{id}"
    return requests.get(url, verify=False)

def post_complaint(data = "", headers = ""):
    url = f"{base_url}/complaints"
    return requests.post(url, json=data, headers=headers, verify=False)

def put_complaint(id = "", data = "", headers= ""):
    url = f"{base_url}/complaints/{id}"
    return requests.put(url, json=data, headers=headers, verify=False)

def delete_complaint(id = "", headers=""):
    url = f"{base_url}/complaints/{id}"
    return requests.delete(url, headers=headers, verify=False)

def search_complaints(urlParams = ""):
    url = f"{base_url}/complaints/search?{urlParams}"
    return requests.get(url, headers={"accept": "text/plain"},verify=False)

def filter_complaints(urlParams = ""):
    url = f"{base_url}/complaints/filter?{urlParams}"
    return requests.get(url, headers={"accept": "text/plain"},verify=False)


# utility functions
def assert_object_equal(requestData, responseData):
    assert requestData.get("productId") == responseData.get("productId")
    assert requestData.get("customer").get("email") == responseData.get("customer").get("email")
    assert requestData.get("customer").get("name") == responseData.get("customer").get("name")
    assert requestData.get("date") == responseData.get("date")
    assert requestData.get("description") == responseData.get("description")
    assert requestData.get("status") == responseData.get("status")

def printComplaints(collection):
    maxlines = 3
    for element in collection[:maxlines]:
        print(element)
    if (len(collection) > maxlines):
        print("...\n")

def find(collection, id):
    for element in collection:
        if element.get("id") == id:
            return element
    return None

# create new complaint, and check if its obtainable via both types of get calls
def createAndVerify(complaint = ""):
    response = post_complaint(data=complaint, headers=putpost_headers_admin)
    assert response.status_code == 201, f"Assert failed with status code {response.status_code}"
    json = response.json()
    print(f"Performed POST with\n{complaint}\nand got:\n{json}")
    id = json.get("id")
    assert_object_equal(complaint, json)
    print(f"Response object has the same data as the one we sent: OK. Id is {id}")

    response = get_complaints()
    assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
    complaints = response.json()
    print("Performed GET on all complaints and got:")
    printComplaints(complaints)
    myObject = find(complaints, id)
    assert_object_equal(complaint, myObject)
    print("Found an object with the id we received and tested for equality: OK")


    response = get_complaints(id=id)
    assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
    json = response.json()
    print(f"Performed GET on our object with id {id} and got:\n{json}")
    assert_object_equal(complaint, json)
    print(f"The object matches with the one we created.")

    return id

# start with get all
response = get_complaints()
assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
complaints = response.json()
print("Performed GET on all complaints and got:")
printComplaints(complaints)

# try to post without auth token
response = post_complaint(data=complaint1, headers={"Content-Type": "application/json"})
assert response.status_code == 401, f"Assert failed with status code {response.status_code}"
print("Performed POST without a token got 401 unauthorized:")


# attempt to login with invalid login data
response = login(data=invalidUser)
assert response.status_code == 401, f"Assert failed with status code {response.status_code}"
print("Performed a LOGIN without a token got 401 unauthorized.")

# login as user and receive token
response = login(data=loginUser)
assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
json = response.json()
userToken = json.get("token")
print(f"Performed LOGIN and Received user token: {userToken}")

# define auth headers for user
putpost_headers_user = {
    "Authorization": f"Bearer {userToken}",
    "Content-Type": "application/json"
}

delete_headers_user = {
    "Authorization": f"Bearer {userToken}",
}

# attempt to post with user role
response = post_complaint(data=complaint1, headers=putpost_headers_user)
assert response.status_code == 403, f"Assert failed with status code {response.status_code}"
print("Performed POST as 'user' and got forbidden.")

# login as admin and receive token
response = login(data=loginAdmin)
assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
json = response.json()
adminToken = json.get("token")
print(f"Performed LOGIN and Received admin token: {adminToken}")

# define auth headers for admin
putpost_headers_admin = {
    "Authorization": f"Bearer {adminToken}",
    "Content-Type": "application/json"
}

delete_headers_admin = {
    "Authorization": f"Bearer {adminToken}",
}

# create with admin token
id1 = createAndVerify(complaint1)

# change object and put for comitting the changes
complaint1["description"] = "Das Produkt ist anders als in der Beschreibung"
complaint1["status"] = "InProgress"
response = put_complaint(id = id1, data=complaint1, headers=putpost_headers_admin)
assert response.status_code == 204, f"Assert failed with status code {response.status_code}"
print(f"Performed POST with\n{complaint1}\nand got no content.")

# see if the changes are reflected
response = get_complaints(id=id1)
assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
json = response.json()
print(f"Performed GET on our updated object with id {id1} and got:\n{json}")
assert_object_equal(complaint1, json)
print(f"The object reflects the updated we made.")

# delete (cancel) complaint
response = delete_complaint(id=id1, headers=putpost_headers_admin)
assert response.status_code == 204, f"Assert failed with status code {response.status_code}"
print(f"Performed DELETE on our updated object with id {id1} and got no content.")

# see if the status got updated to Canceled
response = get_complaints(id=id1)
assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
json = response.json()
print(f"Performed GET on our deleted object with id {id} and got:\n{json}")
complaint1["status"] = "Canceled"
assert_object_equal(complaint1, json)
print(f"The object reflects the change we made.")

# create some complaints for search and filter tests
id2 = createAndVerify(complaint3)
id3 = createAndVerify(complaint3)
id4 = createAndVerify(complaint4)
id5 = createAndVerify(complaint5)
id6 = createAndVerify(complaint6)

# search for complaints with productId 101 (=> No. 1, 3 and 6) and verify.
# Ignore if there are other matches due to entries in the db. We just check the ids of our objects.
search_param="productId=101"
response = search_complaints(search_param)
assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
json = response.json()
print(f"Performed search (GET) with parameter {search_param}and got OK.")
searchDto = json.get("searchDto")
print(f"Search object:\n{searchDto}")
assert("101" == searchDto["productId"])
complaints = json.get("complaints")
print(f"Search results:")
printComplaints(complaints)
assert len(complaints) >= 3
complaint = find(complaints, id1)
assert_object_equal(complaint1, complaint)

complaint = find(complaints, id3)
assert_object_equal(complaint3, complaint)

complaint = find(complaints, id6)
assert_object_equal(complaint6, complaint)

## Filter for complaints from 2023 with status InProgress (=> No. 3, 5) and verify.
# Ignore if there are other matches due to entries in the db. We just check the ids of our objects.
search_param="date=2023&status=inprogress"
response = filter_complaints(search_param)
assert response.status_code == 200, f"Assert failed with status code {response.status_code}"
json = response.json()
print(f"Performed filter (GET) with parameter {search_param}and got OK.")
searchDto = json.get("searchDto")
print(f"Search object:\n{searchDto}")
assert("2023" == searchDto["date"])
assert("inprogress" == searchDto["status"])
complaints = json.get("complaints")
print(f"Search results:")
printComplaints(complaints)
assert len(complaints) >= 2
complaint = find(complaints, id3)
assert_object_equal(complaint3, complaint)

complaint = find(complaints, id5)
assert_object_equal(complaint5, complaint)

# Try various error cases
# Not existing complaint id => 404 Not Found
invalidId = -1
response = get_complaints(id = invalidId)
assert response.status_code == 404, f"Assert failed with status code {response.status_code}"
print(f"Performed GET on an invalid ID and got Not Found.")

response = put_complaint(id = invalidId, data=complaint1, headers=putpost_headers_admin)
assert response.status_code == 404, f"Assert failed with status code {response.status_code}"
print(f"Performed PUT on an invalid ID and got Not Found.")

response = delete_complaint(id = invalidId, headers=putpost_headers_admin)
assert response.status_code == 404, f"Assert failed with status code {response.status_code}"
print(f"Performed DELETE on an invalid ID and got Not Found.")

search_param="productId=-1"
response = search_complaints(search_param)
assert response.status_code == 404, f"Assert failed with status code {response.status_code}"
print(f"Performed a search with no results and got not found.")

search_param="productId=-1"
response = filter_complaints(search_param)
assert response.status_code == 404, f"Assert failed with status code {response.status_code}"
print(f"Performed a filter with no results and got not found.")

# Put/delete without token or with just user role => 401 unauthorized or 403 forbidden.
response = put_complaint(id = id1, data=complaint1)
assert response.status_code == 401, f"Assert failed with status code {response.status_code}"
print(f"Performed PUT without authentication token and got 401 unauthorized.")

response = put_complaint(id = id1, data=complaint1, headers=putpost_headers_user)
assert response.status_code == 403, f"Assert failed with status code {response.status_code}"
print(f"Performed PUT without just a user token and got 403 forbidden.")

response = delete_complaint(id = id1)
assert response.status_code == 401, f"Assert failed with status code {response.status_code}"
print(f"Performed DELETE without authentication token and got 401 unauthorized.")

response = delete_complaint(id = id1, headers=putpost_headers_user)
assert response.status_code == 403, f"Assert failed with status code {response.status_code}"
print(f"Performed DELETE without just a user token and got 401 forbidden.")

print("")
print("+++++++++++++++++++++++++++++++++")
print("+++End-to-end-test successful!+++")
print("+++++++++++++++++++++++++++++++++")
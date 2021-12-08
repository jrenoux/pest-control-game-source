import pymongo

if __name__ == "__main__":
    client = pymongo.MongoClient("mongodb+srv://jennifer:FqDRKbCxwJ99ZSLyLzRE@cluster0.lglsj.mongodb.net/GameLogDB?retryWrites=true&w=majority")
    db = client['GameLogDB']
    print(db)

    collection = db['GameLogCollection']

    filter={
        'prolificId': 'JINYyAQmfP4zroEpWIaVFf7D'
    }

    result = client['GameLogDB']['GameLogCollection'].find(
    filter=filter
    )

    for doc in result:
        print(doc)





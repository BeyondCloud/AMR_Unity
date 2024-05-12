import requests
# read ip and port from \AMR_Unity3D_Data\config.json
def get_config():
    import json
    with open(r"./AMR_Unity3D_Data/config.json") as f:
        data = json.load(f)
    return data

class AmrDynamicMethodCaller:
    def __init__(self, ip:str="0.0.0.0", port:int=8000):
        self.ip = ip
        self.port = port
    def __getattr__(self, method_name):
        def method(*args, **kwargs):
            assert(len(args)) < 2
            data=""
            if len(args) == 1:
                data=args[0]
            url = f'http://{self.ip}:{self.port}/{method_name}'
            print(f"Calling {url} with data {data}")
            return requests.post(url, data=str(data))
        return method
config = get_config()
amr = AmrDynamicMethodCaller(ip=config["serverIP"],port=config["serverPort"])

while True:
    cmd = input("Enter command (q to exit): ")
    if cmd == "q":
        break
    if not cmd.startswith("amr."):
        print("command must start with amr.")
        continue
    exec(cmd)



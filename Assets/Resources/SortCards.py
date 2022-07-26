import os
import shutil

def copyFolderStructure(src, dest):
    src = os.path.abspath(src)

    src_prefix = len(src) + len(os.path.sep)

    for root, dirs, files in os.walk(src):
        for dirname in dirs:
            dirpath = os.path.join(dest, root[src_prefix:], dirname)
            print(root[src_prefix:])
            print(dirname)
            os.mkdir(dirpath)

            srcdir = os.path.join(root, dirname)
            files = [f for f in os.listdir(srcdir) if os.path.isfile(os.path.join(srcdir, f))]
            #print(f'list of files in "{srcdir}" is:\n{files}')
            #break
            for file in files:
                imgFileName = file[:-5] + '.png'
                imgCurrPath = os.path.join(dest, imgFileName)
                destImgPath = os.path.join(dirpath, imgFileName)
                #print(f'checking if "{imgCurrPath}" is a file')
                print(imgCurrPath)
                #break
                if os.path.isfile(imgCurrPath):
                    shutil.copyfile(imgCurrPath, destImgPath)

            #break
        #break


copyFolderStructure(r"C:\Users\eimti\Documents\GitHub\Kompas\Assets\Resources\Card Jsons", r"C:\Users\eimti\Documents\GitHub\Kompas\Assets\Resources\Simple Sprites")
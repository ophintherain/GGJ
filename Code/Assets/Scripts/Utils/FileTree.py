import os

def print_tree(path, prefix="", out_lines=None):
    if out_lines is None:
        out_lines = []

    # 获取当前目录下的所有文件和文件夹
    files = os.listdir(path)
    files.sort()

    for index, name in enumerate(files):
        # 如果是 .meta 文件，跳过
        if name.endswith(".meta"):
            continue

        full_path = os.path.join(path, name)
        connector = "└── " if index == len(files) - 1 else "├── "
        out_lines.append(prefix + connector + name)

        if os.path.isdir(full_path):
            # 如果是文件夹，递归处理
            extension = "    " if index == len(files) - 1 else "│   "
            print_tree(full_path, prefix + extension, out_lines)

    return out_lines

# 打印并保存输出
lines = print_tree("./Assets")

# 保存到 tree.txt 文件
with open("tree.txt", "w", encoding="utf-8") as f:
    f.write("\n".join(lines))

print("已输出到 tree.txt（忽略了 .meta 文件）")

